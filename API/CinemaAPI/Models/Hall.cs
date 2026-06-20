using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models
{
    public class Hall
    {
        [Key]
        public int HallID { get; set; }
        public string HallName { get; set; }
        public int RowsCount { get; set; }
        public int SeatsPerRow { get; set; }
        public string HallType { get; set; }
    }
}
