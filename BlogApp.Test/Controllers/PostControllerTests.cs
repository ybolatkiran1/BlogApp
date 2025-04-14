using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogApp.Controllers;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using BlogApp.Tests.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace BlogApp.Tests.Controllers
{
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            _postRepository = new Mock<IPostRepository>();
            _commentRepository = new Mock<ICommentRepository>();
            _userRepository = new Mock<IUserRepository>();

            _controller = new PostController(
                _postRepository.Object,
                _commentRepository.Object,
                _userRepository.Object
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();

            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            mockDbSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockDbSet;
        }

        private class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<T>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var result = Execute(expression);
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(Task).GetMethod(nameof(Task.FromResult))
                    ?.MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { result });
                return (TResult)executionResult;
            }
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        [Fact]
        public async Task Index_ShouldReturnPaginatedPosts()
        {
            var posts = new List<Post>
            {
                new Post { PostId = 1, Title = "Post 1", Content = "Content 1", IsActive = true },
                new Post { PostId = 2, Title = "Post 2", Content = "Content 2", IsActive = true }
            };

            var mockPostDbSet = CreateMockDbSet(posts);
            _postRepository.Setup(x => x.PostsWithTags).Returns(mockPostDbSet.Object);
            _postRepository.Setup(x => x.Posts).Returns(mockPostDbSet.Object);

            var result = await _controller.Index(null, 1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PostViewModel>(viewResult.Model);
            Assert.Equal(2, model.Posts.Count);
        }

        [Fact]
        public async Task Create_Get_ShouldReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_WithValidData_ShouldCreatePost()
        {
            var post = new CreatePostViewModel
            {
                Title = "New Post",
                Content = "Content",
                Description = "Description",
                Url = "new-post"
            };

            _postRepository.Setup(x => x.CreatePost(It.IsAny<Post>())).Verifiable();
            _postRepository.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            var result = await _controller.Create(post);

            _postRepository.Verify(x => x.CreatePost(It.IsAny<Post>()), Times.Once);
            _postRepository.Verify(x => x.SaveAsync(), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ShouldReturnView()
        {
            var post = new Post { PostId = 1, Title = "Test Post", UserId = 1, Url = "test-post" };
            var mockPostDbSet = CreateMockDbSet(new List<Post> { post });
            _postRepository.Setup(x => x.Posts).Returns(mockPostDbSet.Object);

            var result = await _controller.Edit("test-post");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PostEditViewModel>(viewResult.Model);
            Assert.Equal(post.Title, model.Title);
        }

        

        
        [Fact]
        public async Task Delete_WithValidId_ShouldDeletePost()
        {
            var post = new Post { PostId = 1, Title = "Test Post", UserId = 1 };
            var mockPostDbSet = CreateMockDbSet(new List<Post> { post });
            _postRepository.Setup(x => x.Posts).Returns(mockPostDbSet.Object);
            _postRepository.Setup(x => x.DeletePost(It.IsAny<Post>())).Verifiable();
            _postRepository.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            var result = await _controller.Delete(1);

            _postRepository.Verify(x => x.DeletePost(It.IsAny<Post>()), Times.Once);
            _postRepository.Verify(x => x.SaveAsync(), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Details_WithValidUrl_ShouldReturnView()
        {
            var post = new Post
            {
                PostId = 1,
                Title = "Test Post",
                Url = "test-post",
                User = new User { UserName = "testuser" }
            };
            var mockPostDbSet = CreateMockDbSet(new List<Post> { post });
            _postRepository.Setup(x => x.Posts).Returns(mockPostDbSet.Object);

            var result = await _controller.Details("test-post");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Post>(viewResult.Model);
            Assert.Equal(post.Title, model.Title);
        }

        [Fact]
        public async Task ToggleStatus_WithValidId_ShouldTogglePostStatus()
        {
            var post = new Post { PostId = 1, Title = "Test Post", UserId = 1, IsActive = true };
            var mockPostDbSet = CreateMockDbSet(new List<Post> { post });
            _postRepository.Setup(x => x.Posts).Returns(mockPostDbSet.Object);
            _postRepository.Setup(x => x.EditPost(It.IsAny<Post>())).Verifiable();
            _postRepository.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            var result = await _controller.ToggleStatus(1);

            _postRepository.Verify(x => x.EditPost(It.IsAny<Post>()), Times.Once);
            _postRepository.Verify(x => x.SaveAsync(), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Profile", redirectResult.ActionName);
        }
    }
} 