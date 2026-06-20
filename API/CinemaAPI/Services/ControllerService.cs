using CinemaAPI.Connection;
using CinemaAPI.Interfaces;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services
{
    public class ControllerService : IControllerService
    {
        private readonly ContextDb _context;

        public ControllerService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> CheckTicket(CheckTicketModel model)
        {
            if (string.IsNullOrWhiteSpace(model.QrCode))
                return new BadRequestObjectResult(new { status = false, message = "Пустой код" });

            var ticket = await _context.Tickets
                .Include(t => t.Show).ThenInclude(s => s.Movie)
                .Include(t => t.Show).ThenInclude(s => s.Hall)
                .FirstOrDefaultAsync(t => t.QrCode == model.QrCode);

            if (ticket == null)
                return new NotFoundObjectResult(new { status = false, message = "Билет не найден" });

            if (ticket.Status == "возврат")
                return new BadRequestObjectResult(new { status = false, message = "Билет возвращён" });

            if (ticket.IsCheckedIn)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Билет уже использован {ticket.CheckedInAt:dd.MM.yyyy HH:mm}"
                });
            }

            ticket.IsCheckedIn = true;
            ticket.CheckedInAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Проход разрешён",
                ticket = new
                {
                    ticket.TicketID,
                    Movie = ticket.Show.Movie.Title,
                    Hall = ticket.Show.Hall.HallName,
                    ticket.Show.StartTime,
                    Place = $"Ряд {ticket.RowNumber}, место {ticket.SeatNumber}"
                }
            });
        }

        public async Task<IActionResult> GetTodayShows()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var list = await _context.Shows
                .Include(x => x.Movie)
                .Include(x => x.Hall)
                .Where(x => x.StartTime >= today && x.StartTime < tomorrow)
                .OrderBy(x => x.StartTime)
                .Select(x => new
                {
                    x.ShowID,
                    MovieTitle = x.Movie.Title,
                    HallName = x.Hall.HallName,
                    x.StartTime,
                    x.Status
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, shows = list });
        }

        public async Task<IActionResult> GetAttendance(int showID)
        {
            var sold = await _context.Tickets.CountAsync(t => t.ShowID == showID && t.Status != "возврат");
            var checkedIn = await _context.Tickets.CountAsync(t => t.ShowID == showID && t.IsCheckedIn);
            return new OkObjectResult(new
            {
                status = true,
                attendance = new { sold, checkedIn, missed = sold - checkedIn }
            });
        }
    }
}
