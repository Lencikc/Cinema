using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaAPI.Models
{
    public class BarOrderItem
    {
        [Key]
        public int BarOrderItemID { get; set; }

        [ForeignKey("BarOrder")]
        public int BarOrderID { get; set; }
        public BarOrder BarOrder { get; set; }

        [ForeignKey("BarItem")]
        public int BarItemID { get; set; }
        public BarItem BarItem { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
