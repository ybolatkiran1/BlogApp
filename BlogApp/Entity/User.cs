using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace BlogApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        
        [Required]
        public string? UserName { get; set; }
        
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Surname { get; set; }
        
        [Required]
        public string? Email { get; set; }
        
        [Required]
        public string? Password { get; set; }
        
        public string? Image { get; set; }
        
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
