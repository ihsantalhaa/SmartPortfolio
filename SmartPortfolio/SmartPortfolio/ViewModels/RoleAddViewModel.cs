using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.ViewModels
{
    public class RoleAddViewModel
    {
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role name can't be null !!!")]
        required public string RoleName { get; set; }
    }
}
