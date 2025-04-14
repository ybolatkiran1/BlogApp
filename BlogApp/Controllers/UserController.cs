using DataAccessLayer.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogApp.Models;
using System.Security.Claims;
using BlogApp.Entity;
using BlogApp.Data.Abstract;
using BlogApp.Services;

namespace BlogApp.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        public UserController(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","User");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.Username || x.Email == model.Email);
                if (user == null)
                {
                    _userRepository.CreateUser(new Entity.User
                    {
                        UserName = model.Username,
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        Password = PasswordHasher.HashPassword(model.Password),
                        Image = "default.jpg"
                    });
                    await _userRepository.SaveAsync();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Username ya da Email kullanımda.");
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                
                if (user != null)
                {
                    // Eski kullanıcılar için normal şifre kontrolü
                    bool isPasswordValid = false;
                    
                    // Eğer şifre hashlenmişse (yeni kayıt)
                    if (user.Password.Length == 44) // Base64 hash uzunluğu
                    {
                        isPasswordValid = PasswordHasher.VerifyPassword(model.Password, user.Password);
                    }
                    // Eski kullanıcılar için normal şifre kontrolü
                    else
                    {
                        isPasswordValid = user.Password == model.Password;
                    }

                    if (isPasswordValid)
                    {
                        var userClaims = new List<Claim>();

                        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
                        userClaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
                        userClaims.Add(new Claim(ClaimTypes.GivenName, user.Name ?? ""));
                        userClaims.Add(new Claim(ClaimTypes.UserData, user.Image ?? ""));

                        if (user.Email == "info@info.com")
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                        }

                        var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authPoperties = new AuthenticationProperties { IsPersistent = true };

                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), authPoperties
                        );

                        return RedirectToAction("Index", "Post");
                    }
                }
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
            }
            return View(model);
        }
        public async Task<IActionResult> Profile(string username, int page = 1)
        {
            const int pageSize = 4;

            var user = await _userRepository.Users
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Comments)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            var totalPosts = user.Posts.Count;
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            var paginatedPosts = user.Posts
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new UserProfileViewModel
            {
                UserName = user.UserName!,
                Name = user.Name!,
                Surname = user.Surname!,
                Image = user.Image,
                Posts = paginatedPosts,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalPostCount = totalPosts
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                UserName = user.UserName,
                Image = user.Image
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model, IFormFile? imageFile)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if username is unique if changed
                if (user.UserName != model.UserName)
                {
                    var existingUser = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten kullanımda.");
                        return View(model);
                    }
                }

                // Check if email is unique if changed
                if (user.Email != model.Email)
                {
                    var existingUser = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Bu email adresi zaten kullanımda.");
                        return View(model);
                    }
                }

                // Şifre değişikliği için mevcut şifre kontrolü
                if (!string.IsNullOrEmpty(model.CurrentPassword) || !string.IsNullOrEmpty(model.NewPassword) || !string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    if (string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        ModelState.AddModelError("CurrentPassword", "Mevcut şifrenizi girmelisiniz.");
                        return View(model);
                    }

                    // Eski ve yeni şifre formatı kontrolü
                    bool isPasswordValid = false;
                    
                    // Eğer şifre hashlenmişse (yeni kayıt)
                    if (user.Password.Length == 44) // Base64 hash uzunluğu
                    {
                        isPasswordValid = PasswordHasher.VerifyPassword(model.CurrentPassword, user.Password);
                    }
                    // Eski kullanıcılar için normal şifre kontrolü
                    else
                    {
                        isPasswordValid = user.Password == model.CurrentPassword;
                    }

                    if (!isPasswordValid)
                    {
                        ModelState.AddModelError("CurrentPassword", "Mevcut şifreniz yanlış.");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.NewPassword))
                    {
                        ModelState.AddModelError("NewPassword", "Yeni şifrenizi girmelisiniz.");
                        return View(model);
                    }

                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Yeni şifreler eşleşmiyor.");
                        return View(model);
                    }

                    user.Password = PasswordHasher.HashPassword(model.NewPassword);
                }

                // Update user data
                user.Name = model.Name;
                user.Surname = model.Surname;
                user.Email = model.Email;
                user.UserName = model.UserName;

                // Process image upload if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Get file extension
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}{extension}";

                    // Save file path
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "users", fileName);

                    // Create directory if not exists
                    var directory = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory!);
                    }

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    user.Image = fileName;
                }

                _userRepository.UpdateUser(user);
                await _userRepository.SaveAsync();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "User");
            }

            return View(model);
        }

        private async Task UpdateUserCookie(User user)
        {
            // Sign user out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Create new claims
            var userClaims = new List<Claim>();
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            userClaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
            userClaims.Add(new Claim(ClaimTypes.GivenName, user.Name ?? ""));
            userClaims.Add(new Claim(ClaimTypes.UserData, user.Image ?? ""));

            if (user.Email == "info@info.com")
            {
                userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
            }

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            // Sign in with updated claims
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }

        public async Task<IActionResult> Authors()
        {
            var users = await _userRepository.Users
                .Include(u => u.Posts.Where(p => p.IsActive == true))  // Sadece aktif postları dahil et
                .Select(u => new UserProfileViewModel
                {
                    UserName = u.UserName,
                    Name = u.Name,
                    Image = u.Image,
                    TotalPostCount = u.Posts.Count(p => p.IsActive == true),  // Aktif post sayısını say
                    Posts = u.Posts.Where(p => p.IsActive == true)  // Aktif postları filtrele
                              .OrderByDescending(p => p.PublishedOn)
                              .Take(3)
                              .ToList()
                })
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfile()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.Users
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcının tüm yazılarını sil
            foreach (var post in user.Posts.ToList())
            {
                _postRepository.DeletePost(post);
            }

            // Kullanıcının tüm yorumlarını sil
            foreach (var comment in user.Comments.ToList())
            {
                _commentRepository.DeleteComment(comment);
            }

            // Kullanıcıyı sil
            _userRepository.DeleteUser(user);
            await _userRepository.SaveAsync();

            // Çıkış yap
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Post");
        }
    }
}






