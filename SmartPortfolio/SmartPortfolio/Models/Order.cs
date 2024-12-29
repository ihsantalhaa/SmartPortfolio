using System.ComponentModel.DataAnnotations;

namespace SmartPortfolio.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int AssetId { get; set; }
        public Asset? Asset { get; set; }
        public required string OrderType { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public required DateTime Date { get; set; }
    }
}
