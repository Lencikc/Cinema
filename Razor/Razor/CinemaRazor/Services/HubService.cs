using Microsoft.AspNetCore.SignalR.Client;

namespace CinemaRazor.Services
{
    public class HubService : IAsyncDisposable
    {
        private HubConnection? _conn;
        public event Action<int, int, int>? OnSeatTaken;
        public event Action<string>? OnNotice;

        public async Task ConnectAsync()
        {
            if (_conn != null) return;
            _conn = new HubConnectionBuilder()
                .WithUrl("http://localhost:5190/cinemaHub")
                .WithAutomaticReconnect()
                .Build();

            _conn.On<int, int, int>("SeatTaken", (showId, row, seat) => {
                OnSeatTaken?.Invoke(showId, row, seat);
                OnNotice?.Invoke($"Билет куплен: ряд {row}, место {seat}");
            });

            await _conn.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_conn != null)
            {
                await _conn.DisposeAsync();
                _conn = null;
            }
        }
    }
}
