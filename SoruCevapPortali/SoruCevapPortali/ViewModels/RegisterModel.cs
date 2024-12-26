using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortali.ViewModels
{
    public class RegisterModel  
    {
        [Required(ErrorMessage = "Bu alan zorunludur!")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage ="Bu alan zorunludur!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifreleriniz uyuşmuyor!")]
        public string PasswordConfirm { get; set; } = null!;


    }
}
