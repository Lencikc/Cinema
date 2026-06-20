namespace CinemaRazor.Models
{
    public class HallDto
    {
        public int HallID { get; set; }
        public string HallName { get; set; } = "";
        public int RowsCount { get; set; }
        public int SeatsPerRow { get; set; }
        public string HallType { get; set; } = "";
    }
}
