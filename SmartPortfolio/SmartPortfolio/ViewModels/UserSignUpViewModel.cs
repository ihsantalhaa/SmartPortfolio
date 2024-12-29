using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.ViewModels
{
    public class UserSignUpViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username can't null !!!")]
        required public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email can't null !!!")]
        [EmailAddress(ErrorMessage = "Enter a real mail adress !!!")]
        required public string Email { get; set; }

        [Display(Name = "PhoneNumber")]
        [Phone(ErrorMessage = "Enter a phone number !!!")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(60, MinimumLength = 6, ErrorMessage = "Password need min 6 character !!!")]
        [Required(ErrorMessage = "Password Need Min 6 Character, Upper Case, Number and Alphanumeric Character !!!")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[A-Z])(?=.*[\W_]).+$", ErrorMessage = "Must contain at least one number, uppercase and alphanumeric character !!!")]
        required public string Password { get; set; }

        [Display(Name = "Password Validate")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Validate Can Not Null !!!")]
        [Compare("Password", ErrorMessage = "Passwords Not Match !!!")]
        public string? ConfirmPassword { get; set; }
    }
}
