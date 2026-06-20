namespace CinemaAPI.Requests
{
    public class AddHallModel
    {
        public string HallName { get; set; }
        public int RowsCount { get; set; }
        public int SeatsPerRow { get; set; }
        public string HallType { get; set; }
    }
}
