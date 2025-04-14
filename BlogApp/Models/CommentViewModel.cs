using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class CommentViewModel
    {
        [Required(ErrorMessage = "Yorum alanı zorunludur.")]
        [Display(Name = "Yorum")]
        public string Text { get; set; } = string.Empty;

        public int PostId { get; set; }
        public DateTime PublishedOn { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserImage { get; set; } = string.Empty;
    }
} 