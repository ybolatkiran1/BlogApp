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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BlogApp.Tests.Controllers
{
    public class CommentControllerTests
    {
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            _commentRepository = new Mock<ICommentRepository>();
            _userRepository = new Mock<IUserRepository>();

            _controller = new CommentController(
                _commentRepository.Object,
                _userRepository.Object
            );

            // Setup default HttpContext with authenticated user
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
                if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultType = typeof(TResult).GetGenericArguments()[0];
                    var taskCompletionSourceType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
                    var taskCompletionSource = Activator.CreateInstance(taskCompletionSourceType);
                    taskCompletionSourceType.GetMethod("SetResult")?.Invoke(taskCompletionSource, new[] { result });
                    return (TResult)taskCompletionSourceType.GetProperty("Task")?.GetValue(taskCompletionSource);
                }
                else if (typeof(TResult) == typeof(Task))
                {
                    return (TResult)(object)Task.CompletedTask;
                }
                else
                {
                    var taskCompletionSource = new TaskCompletionSource<TResult>();
                    taskCompletionSource.SetResult((TResult)result);
                    return (TResult)(object)taskCompletionSource.Task;
                }
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

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();

            // Setup IQueryable
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Setup IAsyncEnumerable
            mockDbSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            return mockDbSet;
        }

        [Fact]
        public async Task AddComment_WhenUserIsAuthenticated_ShouldCreateComment()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "testuser" };
            var mockUserDbSet = CreateMockDbSet(new List<User> { user });
            _userRepository.Setup(x => x.Users).Returns(mockUserDbSet.Object);
            _commentRepository.Setup(x => x.CreateComment(It.IsAny<Comment>())).Verifiable();
            _commentRepository.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _controller.AddComment(1, "Test comment");

            // Assert
            _commentRepository.Verify(x => x.CreateComment(It.IsAny<Comment>()), Times.Once);
            _commentRepository.Verify(x => x.SaveAsync(), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.True((bool)response.success);
        }

        [Fact]
        public async Task AddComment_WhenUserIsNotAuthenticated_ShouldReturnError()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.AddComment(1, "Test comment");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.False((bool)response.success);
            Assert.Equal("Yorum yapabilmek için giriş yapmalısınız.", (string)response.message);
        }

        [Fact]
        public async Task Delete_WhenUserIsAuthorized_ShouldDeleteComment()
        {
            // Arrange
            var comment = new Comment { CommentId = 1, UserId = 1, Text = "Test comment" };
            var mockCommentDbSet = CreateMockDbSet(new List<Comment> { comment });
            _commentRepository.Setup(x => x.Comments).Returns(mockCommentDbSet.Object);
            _commentRepository.Setup(x => x.DeleteComment(It.IsAny<Comment>())).Verifiable();
            _commentRepository.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _controller.Delete(1, 1);

            // Assert
            _commentRepository.Verify(x => x.DeleteComment(It.IsAny<Comment>()), Times.Once);
            _commentRepository.Verify(x => x.SaveAsync(), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.True((bool)response.success);
        }

        [Fact]
        public async Task Delete_WhenUserIsNotAuthorized_ShouldReturnError()
        {
            // Arrange
            var comment = new Comment { CommentId = 1, UserId = 2, Text = "Test comment", Post = new Post { UserId = 3 } };
            var mockCommentDbSet = CreateMockDbSet(new List<Comment> { comment });
            _commentRepository.Setup(x => x.Comments).Returns(mockCommentDbSet.Object);

            // Act
            var result = await _controller.Delete(1, 1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.False((bool)response.success);
            Assert.Equal("Bu yorumu silme yetkiniz yok.", (string)response.message);
        }

        [Fact]
        public async Task GetComments_ShouldReturnPaginatedComments()
        {
            
            int postId = 1; 

            var comments = new List<Comment>
    {
        new Comment { CommentId = 1, Text = "Comment 1", User = new User { UserName = "user1" }, PostId = postId },
        new Comment { CommentId = 2, Text = "Comment 2", User = new User { UserName = "user2" }, PostId = postId }
    };

            var mockDbSet = CreateMockDbSet(comments);
            _commentRepository.Setup(x => x.Comments).Returns(mockDbSet.Object);

            // Act
            var result = await _controller.GetComments(postId, 1); // pageNumber=1

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(jsonResult.Value));
            Assert.True((bool)response.success);
            var commentsArray = JArray.FromObject(response.comments);
            Assert.Equal(2, commentsArray.Count);
        }
    }
} 