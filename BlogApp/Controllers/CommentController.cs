using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;

        public CommentController(ICommentRepository commentRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<JsonResult> AddComment(int PostId, string Text)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Json(new { success = false, message = "Yorum yapabilmek için giriş yapmalısınız." });
            }

            // Yorum uzunluğu kontrolü
            if (Text.Length > 300)
            {
                return Json(new { success = false, message = "Yorumunuz 300 karakterden uzun olamaz." });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _userRepository.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            var entity = new Comment
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = userId,
                User = user
            };

            _commentRepository.CreateComment(entity);
            await _commentRepository.SaveAsync();

            return Json(new
            {
                success = true,
                comment = new
                {
                    text = entity.Text,
                    publishedOn = entity.PublishedOn,
                    username = user.UserName,
                    avatar = user.Image ?? "default.jpg"
                }
            });
        }

        [HttpGet]
        public IActionResult GetComments(int postId, int page = 1, int pageSize = 5)
        {
            var comments = _commentRepository.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.CommentId,
                    c.Text,
                    c.PublishedOn,
                    username = c.User.UserName,
                    avatar = c.User.Image
                })
                .ToList();

            var totalComments = _commentRepository.Comments.Count(c => c.PostId == postId);
            var totalPages = (int)Math.Ceiling(totalComments / (double)pageSize);

            return Json(new
            {
                comments = comments,
                currentPage = page,
                totalPages = totalPages,
                totalComments = totalComments
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int commentId, int postId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Json(new { success = false, message = "Bu işlem için giriş yapmanız gerekiyor." });
            }

            try
            {
                var comment = await _commentRepository.Comments
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);

                if (comment == null)
                {
                    return Json(new { success = false, message = "Yorum bulunamadı." });
                }

                // Sadece yorum sahibi veya post sahibi yorumu silebilir
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (comment.UserId != userId && comment.Post.UserId != userId)
                {
                    return Json(new { success = false, message = "Bu yorumu silme yetkiniz yok." });
                }

                _commentRepository.DeleteComment(comment);
                await _commentRepository.SaveAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Yorum silinirken bir hata oluştu: " + ex.Message });
            }
        }
    }
} 