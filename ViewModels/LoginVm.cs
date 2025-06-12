// ViewModels/LoginVm.cs
using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.ViewModels
{
    public class LoginVm
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}