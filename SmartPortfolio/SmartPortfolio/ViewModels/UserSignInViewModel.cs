using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.ViewModels
{
    public class UserSignInViewModel
    {
        [EmailAddress]
        required public string Email { get; set; }

        [DataType(DataType.Password)]
        required public string Password { get; set; }
    }
}
