using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models
{
    public class AgeRating
    {
        [Key]
        public int AgeRatingID { get; set; }
        public int MinAge { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
