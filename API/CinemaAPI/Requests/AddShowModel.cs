namespace CinemaAPI.Requests
{
    public class AddShowModel
    {
        public int MovieID { get; set; }
        public int HallID { get; set; }
        public DateTime StartTime { get; set; }
        public decimal BasePrice { get; set; }
    }
}
