using BlogApp.Entity;
using System.Collections.Generic;

namespace BlogApp.Tests.Fixtures
{
    public static class TestData
    {
        public static List<User> GetTestUsers()
        {
            return new List<User>
            {
                new User
                {
                    UserId = 1,
                    UserName = "testuser",
                    Name = "Test User",
                    Email = "test@test.com",
                    Password = "hashedpassword",
                    Image = "test.jpg",
                    Posts = new List<Post>(),
                    Comments = new List<Comment>()
                },
                new User
                {
                    UserId = 2,
                    UserName = "testuser2",
                    Name = "Test User 2",
                    Email = "test2@test.com",
                    Password = "hashedpassword2",
                    Image = "test2.jpg",
                    Posts = new List<Post>(),
                    Comments = new List<Comment>()
                }
            };
        }

        public static List<Post> GetTestPosts()
        {
            return new List<Post>
            {
                new Post
                {
                    PostId = 1,
                    Title = "Test Post 1",
                    Content = "Test Content 1",
                    Url = "test-post-1",
                    IsActive = true,
                    UserId = 1,
                    Tags = new List<Tag>(),
                    Comments = new List<Comment>()
                },
                new Post
                {
                    PostId = 2,
                    Title = "Test Post 2",
                    Content = "Test Content 2",
                    Url = "test-post-2",
                    IsActive = true,
                    UserId = 1,
                    Tags = new List<Tag>(),
                    Comments = new List<Comment>()
                }
            };
        }

        public static Post GetTestPost()
        {
            return new Post
            {
                PostId = 1,
                Title = "Test Post",
                Content = "Test Content",
                Url = "test-post",
                IsActive = true,
                UserId = 1,
                Tags = new List<Tag>(),
                Comments = new List<Comment>()
            };
        }

        public static User GetTestUser()
        {
            return new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@example.com",
                Image = "test.jpg",
                Posts = new List<Post>()
            };
        }

        public static List<Comment> GetTestComments()
        {
            return new List<Comment>
            {
                new Comment
                {
                    CommentId = 1,
                    Text = "Test Comment 1",
                    PostId = 1,
                    UserId = 1
                },
                new Comment
                {
                    CommentId = 2,
                    Text = "Test Comment 2",
                    PostId = 1,
                    UserId = 2
                }
            };
        }

        public static List<Tag> GetTestTags()
        {
            return new List<Tag>
            {
                new Tag { TagId = 1, Text = "Test", Url = "test" },
                new Tag { TagId = 2, Text = "Programming", Url = "programming" },
                new Tag { TagId = 3, Text = "ASP.NET", Url = "aspnet" }
            };
        }
    }
} 