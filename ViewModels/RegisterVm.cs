using System.ComponentModel.DataAnnotations;

namespace WorkshopManager.ViewModels
{
    public class RegisterVm
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}