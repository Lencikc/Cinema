namespace CinemaRazor.Models
{
    public class TicketDto
    {
        public int TicketID { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public decimal Price { get; set; }
        public string QrCode { get; set; } = "";
        public string Status { get; set; } = "";
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public TicketShowDto Show { get; set; } = new();
    }
    public class TicketShowDto
    {
        public int ShowID { get; set; }
        public DateTime StartTime { get; set; }
        public string MovieTitle { get; set; } = "";
        public string HallName { get; set; } = "";
        public string? Poster { get; set; }
    }
}
