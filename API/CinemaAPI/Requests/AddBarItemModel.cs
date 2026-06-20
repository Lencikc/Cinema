namespace CinemaAPI.Requests
{
    public class AddBarItemModel
    {
        public string ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public int BarCategoryID { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockCount { get; set; }
    }
}
