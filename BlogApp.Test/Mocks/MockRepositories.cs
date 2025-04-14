using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Moq;

namespace BlogApp.Tests.Mocks
{
    public static class MockRepositories
    {
        public static Mock<IUserRepository> GetUserRepository()
        {
            var mock = new Mock<IUserRepository>();
            mock.Setup(repo => repo.CreateUser(It.IsAny<User>()));
            mock.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);
            return mock;
        }

        public static Mock<IPostRepository> GetPostRepository()
        {
            var mock = new Mock<IPostRepository>();
            mock.Setup(repo => repo.CreatePost(It.IsAny<Post>()));
            mock.Setup(repo => repo.EditPost(It.IsAny<Post>()));
            mock.Setup(repo => repo.DeletePost(It.IsAny<Post>()));
            mock.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);
            return mock;
        }

        public static Mock<ICommentRepository> GetCommentRepository()
        {
            var mock = new Mock<ICommentRepository>();
            mock.Setup(repo => repo.CreateComment(It.IsAny<Comment>()));
            mock.Setup(repo => repo.DeleteComment(It.IsAny<Comment>()));
            mock.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);
            return mock;
        }
    }
} 