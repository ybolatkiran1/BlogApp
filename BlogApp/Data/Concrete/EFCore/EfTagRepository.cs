using BlogApp.Entity;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;

namespace BlogApp.Data.Concrete.EFCore
{
    public class EfTagRepository : ITagRepository
    {
        private Context _context;
        public EfTagRepository(Context context)
        {
            _context = context;
        }
        public IQueryable<Tag> Tags => _context.Tags;

        public void CreateTag(Tag Tag)
        {
            _context.Tags.Add(Tag);
            _context.SaveChanges();
        }
    }
}