using Microsoft.AspNetCore.SignalR;

namespace CinemaAPI.Hubs
{
    public class CinemaHub : Hub
    {
        public async Task BroadcastSeatTaken(int showID, int row, int seat)
        {
            await Clients.All.SendAsync("SeatTaken", showID, row, seat);
        }
    }
}
