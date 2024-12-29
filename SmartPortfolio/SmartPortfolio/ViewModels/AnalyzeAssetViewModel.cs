namespace SmartPortfolio.ViewModels
{
    public class AnalyzeAssetViewModel
    {
        public int AssetId { get; set; }
        public string? SymbolName { get; set; }
        public string? SymbolType { get; set; }
        public double ClosedPrice { get; set; }
        public double FK { get; set; }
        public double FdFavok { get; set; }
        public double FdSell { get; set; }
        public double PdDd { get; set; }
        public double LastPrice { get; set; }

        public string? Date { get; set; }
        public string? ClosingTl { get; set; }
        public string? LowTl { get; set; }
        public string? HighTl { get; set; }
        public string? VolumeTl { get; set; }
        public string? ClosingUsd { get; set; }
        public string? LowUsd { get; set; }
        public string? HighUsd { get; set; }
        public string? VolumeUsd { get; set; }
        public string? Xu100Tl { get; set; }
        public string? Xu100Usd { get; set; }
    }
}
