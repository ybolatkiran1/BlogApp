using BlogApp.Entity;
using BlogApp.Data.Abstract;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EfPostRepository : IPostRepository
    {
        private Context _context;
        public EfPostRepository(Context context)
        {
            _context = context;
        }

        public IQueryable<Post> Posts => _context.Posts;

        public IQueryable<Post> PostsWithTags =>
            _context.Posts
                .Include(p => p.Tags)
                .Include(p => p.User);

        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void EditPost(Post post)
        {
            var entity = _context.Posts.FirstOrDefault(i => i.PostId == post.PostId);

            if (entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.Content = post.Content;
                entity.Url = post.Url;
                entity.IsActive = post.IsActive;
                entity.Image = post.Image;

                entity.Tags = post.Tags; // Tag güncellemeyi de kapsasın

                _context.SaveChanges();
            }
        }

        public Post GetPostById(int postId)
        {
            return _context.Posts.FirstOrDefault(p => p.PostId == postId);
        }

        // Yeni metodlar
        public async Task<Tag?> GetTagByTextAsync(string text)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Text == text);
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DeletePost(Post post)
        {
            _context.Posts.Remove(post);
            _context.SaveChanges();
        }
        public async Task<bool> PostTitleExistsAsync(string title)
        {
            return await _context.Posts.AnyAsync(p => p.Title.ToLower() == title.ToLower());
        }

    }
}
