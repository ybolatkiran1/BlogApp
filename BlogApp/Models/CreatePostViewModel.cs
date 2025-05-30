using System.ComponentModel.DataAnnotations;
using BlogApp.Entity;

namespace BlogApp.Models
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [Display(Name = "Açıklama")]
        [StringLength(150, ErrorMessage = "Açıklama en fazla 150 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL alanı zorunludur.")]
        [Display(Name = "URL")]
        public string Url { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resim alanı zorunludur.")]
        [Display(Name = "Resim")]
        public IFormFile? Image { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Etiketler")]
        public string TagsInput { get; set; } = string.Empty;

        public List<string> GetParsedTags()
        {
            return TagsInput
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

} 