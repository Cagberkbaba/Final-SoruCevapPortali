using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortali.ViewModels
{
    public class LoginModel
    {
        [Required]
        
        [Display(Name = "Email Veya Kullanıcı Adı :")]
        public string EmailOrName { get; set; } = null!;


        [Display(Name = "Şifre :")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;


        public bool RememberMe { get; set; } = false;
    }
}
