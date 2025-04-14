using BlogApp.Data.Concrete.EFCore;
using System.Security.Claims;
using BlogApp.Entity;
using BlogApp.Models;
using BlogApp.Data.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {

        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private IUserRepository _userRepository;
        public PostController(IPostRepository postRepository, ICommentRepository commentRepository,IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }


        public async Task<IActionResult> Index(string tag, int page = 1)
        {
            const int pageSize = 6;

            var query = _postRepository.PostsWithTags
                .Include(p => p.Comments)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(x => x.Tags.Any(t => t.Url == tag));
            }

            var totalPosts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            var paginatedPosts = await query
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allActiveTags = await _postRepository.PostsWithTags
                .Where(p => p.IsActive)
                .SelectMany(p => p.Tags)
                .GroupBy(t => t.Url)
                .Select(g => g.First())
                .ToListAsync();

            return View(new PostViewModel
            {
                Posts = paginatedPosts,
                CurrentPage = page,
                TotalPages = totalPages,
                Tag = tag,
                AllTags = allActiveTags 
            });
        }

        public async Task<IActionResult> Details(string url)
        {
            var post = await _postRepository.Posts
                .Include(x => x.Tags)
                .Include(x => x.Comments).ThenInclude(x => x.User)
                .Include(x => x.User) 
                .FirstOrDefaultAsync(p => p.Url == url);
           
            return View(post);
        }
        [HttpGet("/Post/Edit/{url}")]
        public async Task<IActionResult> Edit(string url)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            var post = await _postRepository.Posts
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Url == url);

            if (post == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (post.UserId != userId)
            {
                return Unauthorized();
            }

            var postEditViewModel = new PostEditViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Url = post.Url,
                Image = post.Image,
                IsActive = post.IsActive,
                Comments = post.Comments
            };
            postEditViewModel.TagsInput = string.Join(", ", post.Tags.Select(t => t.Text));

            return View("~/Views/Post/Edit.cshtml", postEditViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PostEditViewModel model, IFormFile? image)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                if (await _postRepository.PostTitleExistsAsync(model.Title))
                {
                    ModelState.AddModelError("Title", "Bu başlığa sahip bir yazı zaten mevcut.");
                    return View(model);
                }

                var tagNames = model.TagsInput
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Distinct()
                    .ToList();

                if (tagNames.Count == 0)
                {
                    ModelState.AddModelError("TagsInput", "Lütfen en az bir geçerli etiket giriniz.");
                    return View(model);
                }

                var post = await _postRepository.Posts
                    .Include(p => p.Tags)
                    .FirstOrDefaultAsync(p => p.PostId == model.PostId);

                if (post == null)
                {
                    return NotFound();
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (post.UserId != userId)
                {
                    return Unauthorized();
                }

                post.Title = model.Title;
                post.Content = model.Content;
                post.Description = model.Description;
                post.Url = model.Url;
                post.IsActive = model.IsActive;

                if (image != null)
                {
                    var extension = Path.GetExtension(image.FileName);
                    var randomFileName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/post", randomFileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    post.Image = randomFileName;
                }

                post.Tags.Clear();

                foreach (var tagName in tagNames)
                {
                    var tag = await _postRepository.GetTagByTextAsync(tagName);

                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Text = tagName,
                            Url = tagName.ToLower().Replace(" ", "-")
                        };
                        await _postRepository.AddTagAsync(tag);
                    }

                    post.Tags.Add(tag);
                }

                _postRepository.EditPost(post);
                await _postRepository.SaveAsync();

                return RedirectToAction("Details", new { url = post.Url });
            }

            return View(model);
        }


        [HttpPost]
        [Route("/Post/Delete/{postId}")]
        public async Task<IActionResult> Delete(int postId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            var post = await _postRepository.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (post.UserId != userId)
            {
                return Unauthorized();
            }

            if (post.Comments != null && post.Comments.Any())
            {
                foreach (var comment in post.Comments.ToList())
                {
                    if (comment != null)
                    {
                        _commentRepository.DeleteComment(comment);
                    }
                }
            }

            _postRepository.DeletePost(post);
            await _postRepository.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                if (await _postRepository.PostTitleExistsAsync(model.Title))
                {
                    ModelState.AddModelError("Title", "Bu başlığa sahip bir yazı zaten mevcut.");
                    return View(model);
                }
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    Description = model.Description,
                    Url = model.Url,
                    IsActive = model.IsActive,
                    PublishedOn = DateTime.Now,
                    UserId = userId
                };

                if (model.Image != null)
                {
                    var extension = Path.GetExtension(model.Image.FileName);
                    var randomFileName = $"{Guid.NewGuid()}{extension}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/post", randomFileName);
                    
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory!);
                    }
                    
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                    post.Image = randomFileName;
                }

                if (!string.IsNullOrEmpty(model.TagsInput))
                {
                    var tagNames = model.GetParsedTags();

                    if (tagNames.Count == 0)
                    {
                        ModelState.AddModelError("TagsInput", "Lütfen en az bir geçerli etiket giriniz.");
                        return View(model);
                    }

                    foreach (var tagName in tagNames)
                    {
                        var tag = await _postRepository.GetTagByTextAsync(tagName);

                        if (tag == null)
                        {
                            tag = new Tag
                            {
                                Text = tagName,
                                Url = tagName.ToLower().Replace(" ", "-")
                            };
                            await _postRepository.AddTagAsync(tag);
                        }

                        post.Tags.Add(tag);
                    }
                }

                _postRepository.CreatePost(post);
                await _postRepository.SaveAsync();

                return RedirectToAction("Details", new { url = post.Url });
            }

            return View(model);
        }

        [HttpPost]
        [Route("/Post/ToggleStatus/{postId}")]
        public async Task<IActionResult> ToggleStatus(int postId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            var post = await _postRepository.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (post.UserId != userId)
            {
                return Unauthorized();
            }

            post.IsActive = !post.IsActive;
            _postRepository.EditPost(post);
            await _postRepository.SaveAsync();

            return RedirectToAction("Profile", "User", new { username = User.Identity.Name });
        }
    }
}