using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.ViewModels
{
    public class UserDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username can't be null !!!")]
        public string? Username { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Enter a real mail adress !!!")]
        public string? Email { get; set; }

        [Display(Name = "PhoneNumber")]
        [Phone(ErrorMessage = "Enter a real phone number !!!")]
        public string? PhoneNumber { get; set; }
    }
}
