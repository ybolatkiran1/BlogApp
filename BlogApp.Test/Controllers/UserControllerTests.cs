using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogApp.Controllers;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace BlogApp.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly UserController _controller;
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly Mock<IAuthenticationService> _authService;
        private readonly Mock<ITempDataDictionary> _tempData;
        private readonly Mock<IUrlHelperFactory> _urlHelperFactory;
        private readonly Mock<IUrlHelper> _urlHelper;

        public UserControllerTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _postRepository = new Mock<IPostRepository>();
            _commentRepository = new Mock<ICommentRepository>();
            _tempData = new Mock<ITempDataDictionary>();
            _serviceProvider = new Mock<IServiceProvider>();
            _authService = new Mock<IAuthenticationService>();
            _urlHelperFactory = new Mock<IUrlHelperFactory>();
            _urlHelper = new Mock<IUrlHelper>();

            _serviceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_authService.Object);

            _serviceProvider
                .Setup(x => x.GetService(typeof(IUrlHelperFactory)))
                .Returns(_urlHelperFactory.Object);

            _urlHelperFactory
                .Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(_urlHelper.Object);

            _controller = new UserController(
                _userRepository.Object,
                _postRepository.Object,
                _commentRepository.Object
            )
            {
                TempData = _tempData.Object
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = _serviceProvider.Object
                }
            };
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();
            var mockQueryProvider = new Mock<IAsyncQueryProvider>();

            mockQueryProvider
                .Setup(x => x.ExecuteAsync<Task<T>>(It.IsAny<System.Linq.Expressions.Expression>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns<System.Linq.Expressions.Expression, System.Threading.CancellationToken>((expr, token) => Task.FromResult(queryable.Provider.Execute<T>(expr)));

            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(mockQueryProvider.Object);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockDbSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockDbSet;
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldRedirectToHome()
        {
            
            var user = new User { Email = "test@example.com", Password = "testpass" };
            var mockDbSet = CreateMockDbSet(new List<User> { user });
            _userRepository.Setup(x => x.Users).Returns(mockDbSet.Object);

            
            var result = await _controller.Login(new LoginViewModel { Email = "test@example.com", Password = "testpass" });

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Post", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnViewWithError()
        {
            var user = new User { Email = "test@example.com", Password = "testpass" };
            var mockDbSet = CreateMockDbSet(new List<User> { user });
            _userRepository.Setup(x => x.Users).Returns(mockDbSet.Object);

            var result = await _controller.Login(new LoginViewModel { Email = "wrong@example.com", Password = "wrongpass" });

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ErrorCount > 0);
        }

        [Fact]
        public async Task Register_WithValidData_ShouldCreateUserAndRedirect()
        {
            var registerModel = new RegisterViewModel
            {
                Username = "newuser",
                Name = "New User",
                Email = "newuser@example.com",
                Password = "newpass",
                ConfirmPassword = "newpass"
            };

            var mockDbSet = CreateMockDbSet(new List<User>());
            _userRepository.Setup(x => x.Users).Returns(mockDbSet.Object);
            _userRepository.Setup(x => x.CreateUser(It.IsAny<User>())).Callback<User>(user => {
              
                var users = new List<User> { user };
                var newMockDbSet = CreateMockDbSet(users);
                _userRepository.Setup(x => x.Users).Returns(newMockDbSet.Object);
            });
            _userRepository.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            var result = await _controller.Register(registerModel);

            _userRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Once);
            _userRepository.Verify(x => x.SaveAsync(), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }

        [Fact]
        public async Task Register_WithInvalidData_ShouldReturnViewWithError()
        {
           
            var registerModel = new RegisterViewModel
            {
                Username = "",
                Name = "",
                Email = "invalid-email",
                Password = "pass",
                ConfirmPassword = "differentpass"
            };

            var mockDbSet = CreateMockDbSet(new List<User>());
            _userRepository.Setup(x => x.Users).Returns(mockDbSet.Object);

            
            _controller.ModelState.AddModelError("Email", "Invalid email format");
            _controller.ModelState.AddModelError("Password", "Password must be at least 6 characters");
            _controller.ModelState.AddModelError("ConfirmPassword", "Passwords do not match");

            
            var result = await _controller.Register(registerModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(_controller.ModelState.ErrorCount > 0);
            Assert.Equal(registerModel, viewResult.Model);
        }

        [Fact]
        public async Task Logout_ShouldSignOutAndRedirect()
        {
            _authService.Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()
            )).Returns(Task.CompletedTask);

            var result = await _controller.Logout();

            _authService.Verify(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()
            ), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("User", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Profile_WithValidUsername_ShouldReturnView()
        {
            
            var user = new User { UserName = "testuser", Name = "Test User", Image = "test.jpg" };
            var mockDbSet = CreateMockDbSet(new List<User> { user });
            _userRepository.Setup(x => x.Users).Returns(mockDbSet.Object);

            
            var result = await _controller.Profile("testuser");

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Profile_WithInvalidUsername_ShouldReturnNotFound()
        {
           
            var mockDbSet = CreateMockDbSet(new List<User>());
            _userRepository.Setup(x => x.Users).Returns(mockDbSet.Object);

            
            var result = await _controller.Profile("nonexistentuser");

           
            Assert.IsType<NotFoundResult>(result);
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }
    }
} 