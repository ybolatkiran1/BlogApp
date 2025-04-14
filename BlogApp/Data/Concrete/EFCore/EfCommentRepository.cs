using BlogApp.Data.Abstract;
using BlogApp.Entity;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EfCommentRepository : ICommentRepository
    {
        private readonly Context _context;

        public EfCommentRepository(Context context)
        {
            _context = context;
        }

        public IQueryable<Comment> Comments => _context.Comments;

        public void CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
        }

        public void DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}