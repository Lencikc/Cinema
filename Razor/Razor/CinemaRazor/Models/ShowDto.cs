namespace CinemaRazor.Models
{
    public class ShowDto
    {
        public int ShowID { get; set; }
        public int MovieID { get; set; }
        public string MovieTitle { get; set; } = "";
        public int HallID { get; set; }
        public string HallName { get; set; } = "";
        public string HallType { get; set; } = "";
        public DateTime StartTime { get; set; }
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = "";
    }
}
