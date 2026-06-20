using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaAPI.Models
{
    public class BarItem
    {
        [Key]
        public int BarItemID { get; set; }
        public string ItemName { get; set; }
        public string? ItemDescription { get; set; }

        [ForeignKey("BarCategory")]
        public int BarCategoryID { get; set; }
        public BarCategory? BarCategory { get; set; }

        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockCount { get; set; }
        public bool IsActive { get; set; }
    }
}
