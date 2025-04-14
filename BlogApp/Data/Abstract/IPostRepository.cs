using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IPostRepository
    {
        IQueryable<Post> Posts { get; }
        IQueryable<Post> PostsWithTags { get; }
        void CreatePost(Post post);
        void EditPost(Post post);
        Post GetPostById(int postId);
        Task<Tag?> GetTagByTextAsync(string text);
        Task AddTagAsync(Tag tag);
        Task SaveAsync();
        void DeletePost(Post post);
        Task<bool> PostTitleExistsAsync(string title);

    }
}
