using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models
{
    public class BarCategory
    {
        [Key]
        public int BarCategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
