using BlogApp.Entity;

namespace BlogApp.Models
{
    public class PostViewModel
    {
        public List<Post> Posts { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Tag { get; set; }
        public List<Tag> AllTags { get; set; } = new();
        public int TotalPostCount { get; set; }
    }
}
