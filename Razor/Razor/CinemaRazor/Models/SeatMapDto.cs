namespace CinemaRazor.Models
{
    public class SeatMapDto
    {
        public bool Status { get; set; }
        public HallShortDto Hall { get; set; } = new();
        public decimal BasePrice { get; set; }
        public List<RowDto> Rows { get; set; } = new();
    }
    public class HallShortDto
    {
        public string HallName { get; set; } = "";
        public int RowsCount { get; set; }
        public int SeatsPerRow { get; set; }
    }
    public class RowDto
    {
        public int RowNumber { get; set; }
        public List<SeatDto> Seats { get; set; } = new();
    }
    public class SeatDto
    {
        public int Row { get; set; }
        public int Seat { get; set; }
        public bool Taken { get; set; }
    }
}
