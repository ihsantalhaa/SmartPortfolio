using SmartPortfolio.Models;

namespace SmartPortfolio.ViewModels
{
    public class PortfolioOrdersViewModel
    {
        // Unnecessary but may come in handy in the future
        public int AssetId { get; set; }

        public string? SymbolName { get; set; }
        public string? OrderType { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }

    }
}
