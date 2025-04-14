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

        public async Task<IActionResult> GetComments(int postId, int page = 1)
        {
            try
            {
                const int pageSize = 5;
                var comments = await _commentRepository.Comments
                    .Include(c => c.User)
                    .Where(c => c.PostId == postId)
                    .OrderByDescending(c => c.PublishedOn)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalComments = await _commentRepository.Comments
                    .Where(c => c.PostId == postId)
                    .CountAsync();

                var totalPages = (int)Math.Ceiling(totalComments / (double)pageSize);

                var commentViewModels = comments.Select(c => new
                {
                    text = c.Text,
                    publishedOn = c.PublishedOn,
                    username = c.User?.UserName ?? "Anonim",
                    avatar = c.User?.Image ?? "default.jpg"
                }).ToList();

                return Json(new
                {
                    success = true,
                    comments = commentViewModels,
                    currentPage = page,
                    totalPages = totalPages
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Yorumlar yüklenirken bir hata oluştu: " + ex.Message });
            }
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