using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.ViewModels
{
    public class RoleDetailsViewModel
    {
        required public int Id { get; set; }

        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role name can't null !!!")]
        required public string RoleName { get; set; }
    }
}
