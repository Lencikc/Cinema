using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models
{
    public class Genre
    {
        [Key]
        public int GenreID { get; set; }
        public string GenreName { get; set; } = string.Empty;
    }
}
