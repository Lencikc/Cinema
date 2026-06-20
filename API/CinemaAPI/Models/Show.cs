using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaAPI.Models
{
    public class Show
    {
        [Key]
        public int ShowID { get; set; }

        [ForeignKey("Movie")]
        public int MovieID { get; set; }
        public Movie Movie { get; set; }

        [ForeignKey("Hall")]
        public int HallID { get; set; }
        public Hall Hall { get; set; }

        public DateTime StartTime { get; set; }
        public decimal BasePrice { get; set; }
        public string Status { get; set; }
    }
}
