using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaAPI.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [ForeignKey("Genre")]
        public int GenreID { get; set; }
        public Genre? Genre { get; set; }

        public int DurationMinutes { get; set; }

        [ForeignKey("AgeRating")]
        public int AgeRatingID { get; set; }
        public AgeRating? AgeRating { get; set; }

        public string? PosterUrl { get; set; }
        public int ReleaseYear { get; set; }
        public bool IsActive { get; set; }
    }
}
