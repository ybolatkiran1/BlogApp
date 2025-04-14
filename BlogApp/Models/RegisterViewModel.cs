using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string? Username { get; set; }

    [Required]
    [Display(Name = "Name")]
    [RegularExpression(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$", ErrorMessage = "İsim yalnızca harf içerebilir.")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "Surname")]
    [RegularExpression(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$", ErrorMessage = "Soyisim yalnızca harf içerebilir.")]
    public string? Surname { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Eposta")]
    public string? Email { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "{0} alanı en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Parola")]
    public string? Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Parolanız eşleşmiyor.")]
    [Display(Name = "Parola (Tekrar)")]
    public string? ConfirmPassword { get; set; }
}
