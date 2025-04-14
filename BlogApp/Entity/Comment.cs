using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entity
{
    public class Comment
    {
        public int CommentId { get; set; }
        
        [Required]
        public string? Text { get; set; }
        
        public DateTime PublishedOn { get; set; }
        
        public int PostId { get; set; }
        public Post? Post { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
