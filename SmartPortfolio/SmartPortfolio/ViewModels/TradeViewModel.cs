using SmartPortfolio.Models;

namespace SmartPortfolio.ViewModels
{
    public class TradeViewModel
    {
        public int AssetId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int LastPrice { get; set; }
        public string? SymbolName { get; set; }
        public string? SymbolType { get; set; }
        public int Amount { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
