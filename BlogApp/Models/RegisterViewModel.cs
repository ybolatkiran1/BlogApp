using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string? Username { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Eposta")]
        public string? Email { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "{0} alaný en az {2} karakter uzunluðunda olmalýdýr.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Parolanýz eþleþmiyor.")]
        [Display(Name = "Parola")]
        public string? ConfirmPassword { get; set; }
    }
}
