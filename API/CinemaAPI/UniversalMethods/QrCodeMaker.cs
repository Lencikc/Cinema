namespace CinemaAPI.UniversalMethods
{
    public class QrCodeMaker
    {
        public string Make(int ticketID)
        {
            string raw = $"CINEMA-{ticketID}-{Guid.NewGuid():N}";
            return raw;
        }
    }
}
