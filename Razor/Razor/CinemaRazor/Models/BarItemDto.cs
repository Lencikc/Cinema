namespace CinemaRazor.Models
{
    public class BarItemDto
    {
        public int BarItemID { get; set; }
        public string ItemName { get; set; } = "";
        public string? ItemDescription { get; set; }
        public int BarCategoryID { get; set; }
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockCount { get; set; }
        public bool IsActive { get; set; }
    }
}
