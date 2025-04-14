using System.ComponentModel.DataAnnotations;
using BlogApp.Entity;

public class PostEditViewModel
{
    public int PostId { get; set; }

    [Required(ErrorMessage = "Başlık alanı zorunludur.")]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty; // Null olmayan başlangıç değeri

    [Required(ErrorMessage = "İçerik alanı zorunludur.")]
    [Display(Name = "İçerik")]
    public string Content { get; set; } = string.Empty; // Null olmayan başlangıç değeri

    [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
    [Display(Name = "Açıklama")]
    [StringLength(150, ErrorMessage = "Açıklama en fazla 150 karakter olabilir.")]
    public string Description { get; set; } = string.Empty; // Null olmayan başlangıç değeri

    [Required(ErrorMessage = "URL alanı zorunludur.")]
    [Display(Name = "URL")]
    public string Url { get; set; } = string.Empty; // Null olmayan başlangıç değeri

    [Display(Name = "Resim")]
    public string? Image { get; set; }

    [Display(Name = "Aktif mi?")]
    public bool IsActive { get; set; }

    [Display(Name = "Etiketler")]
    public string TagsInput { get; set; } = string.Empty; 
    public List<string> SelectedTags { get; set; } = new List<string>();

    public List<Comment> Comments { get; set; } = new List<Comment>();
}