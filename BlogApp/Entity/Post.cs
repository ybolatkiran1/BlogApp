using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entity
{
    public class Post
    {
        public int PostId { get; set; }
        
        [Required]
        public string? Title { get; set; }
        
        [Required]
        public string? Content { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public string? Url { get; set; }
        
        public string? Image { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime PublishedOn { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
