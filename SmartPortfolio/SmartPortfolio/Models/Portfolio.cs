using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.Models
{
    public class Portfolio
    {
        [Key]
        public int PortfolioId { get; set; }
        public int UserId { get; set; }
        public required IUser User { get; set; }
        public string? PortfolioName { get; set; }
        public string? PortfolioDescription { get; set; }
        ICollection<Asset>? Assets { get; set; }

    }
}
