using SmartPortfolio.Models;

namespace SmartPortfolio.ViewModels
{
    public class ListAssetsViewModel
    {
        public int AssetId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? LastPrice { get; set; }
        public string? SymbolName { get; set; }
        public string? SymbolType { get; set; }
        public int Amount { get; set; }
        public double Cost { get; set; }

        public string? State { get; set; }
        public double StateValue { get; set; }
        public double StateAmountValue { get; set; }
        public double StateRatio { get; set; }
    }
}
