namespace Nop.Plugin.Widgets.Bronto.Domain
{
    public record BrontoLineItem {
        public string Sku { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Category { get; init; }
        public string Other { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal SalePrice { get; init; }
        public int Quantity { get; init; }
        public decimal TotalPrice { get; init; }
        public string ImageUrl { get; init; }
        public string ProductUrl { get; init; }
    }
}
