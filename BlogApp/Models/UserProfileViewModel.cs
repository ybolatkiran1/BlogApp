using BlogApp.Entity;

namespace BlogApp.Models
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
        public List<Post> Posts { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalPostCount { get; set; }
    }
} 