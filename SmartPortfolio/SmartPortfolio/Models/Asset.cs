using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }
        public int PortfolioId { get; set; }
        public Portfolio? Portfolio { get; set; }
        public DateTime UpdateDate { get; set; }
        public double LastPrice { get; set; }
        public string? SymbolName { get; set; }
        public string? SymbolType { get; set; }
        public int Amount { get; set; }
        public double Cost { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
