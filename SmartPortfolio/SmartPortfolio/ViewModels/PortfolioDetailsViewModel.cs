using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.ViewModels
{
    public class PortfolioDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "PorfolioName")]
        [Required(ErrorMessage = "PorfolioName can't be null !!!")]
        public string? PorfolioName { get; set; }

        [Display(Name = "PorfolioDescription")]
        [Required(ErrorMessage = "PorfolioDescription can't be null !!!")]
        public string? PorfolioDescription { get; set; }

    }
}
