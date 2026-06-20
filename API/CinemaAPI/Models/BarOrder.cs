using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaAPI.Models
{
    public class BarOrder
    {
        [Key]
        public int BarOrderID { get; set; }

        [ForeignKey("User")]
        public int? UserID { get; set; }
        public User? User { get; set; }

        [ForeignKey("Show")]
        public int? ShowID { get; set; }
        public Show? Show { get; set; }

        public decimal TotalSum { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<BarOrderItem> Items { get; set; } = new();
    }
}
