namespace CinemaAPI.Requests
{
    public class BuyTicketModel
    {
        public int ShowID { get; set; }
        public List<SeatPick> Seats { get; set; } = new();
        public string? PromoCode { get; set; }
    }

    public class SeatPick
    {
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
    }
}
