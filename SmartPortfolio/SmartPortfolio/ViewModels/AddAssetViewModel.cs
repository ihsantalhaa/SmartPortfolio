namespace SmartPortfolio.ViewModels
{
    public class AddAssetViewModel
    {
        public int AssetId { get; set; }
        public int PortfolioId { get; set; }
        public string? SymbolName { get; set; }
        public string? SymbolType { get; set; }
        public double ClosedPrice { get; set; }
        public double FK { get; set; }
        public double FdFavok { get; set; }
        public double FdSell { get; set; }
        public double PdDd { get; set; }
        public double LastPrice { get; set; }

    }
}
