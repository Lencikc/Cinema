namespace CinemaAPI.Requests
{
    public class BarOrderModel
    {
        public int? ShowID { get; set; }
        public List<BarOrderLine> Lines { get; set; } = new();
    }

    public class BarOrderLine
    {
        public int BarItemID { get; set; }
        public int Count { get; set; }
    }
}
