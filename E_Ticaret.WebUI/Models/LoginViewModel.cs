using System.ComponentModel.DataAnnotations;

namespace E_Ticaret.WebUI.Models
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress),Required(ErrorMessage ="Email alanı boş geçilemez!")]
        public string Email { get; set; }

        [Display(Name = "Şifre")]
        [DataType(DataType.Password), Required(ErrorMessage = "Şifre alanı boş geçilemez!")]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}
