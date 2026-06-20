using CinemaAPI.Connection;
using CinemaAPI.Hubs;
using CinemaAPI.Interfaces;
using CinemaAPI.Models;
using CinemaAPI.Requests;
using CinemaAPI.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ContextDb _context;
        private readonly QrCodeMaker _qr;
        private readonly IHubContext<CinemaHub> _hub;

        public UserService(ContextDb context, QrCodeMaker qr, IHubContext<CinemaHub> hub)
        {
            _context = context;
            _qr = qr;
            _hub = hub;
        }

        public async Task<IActionResult> BuyTickets(int userID, BuyTicketModel model)
        {
            if (model.Seats == null || model.Seats.Count == 0)
            {
                return new BadRequestObjectResult(new { status = false, message = "Не выбраны места" });
            }

            var show = await _context.Shows.Include(x => x.Hall).FirstOrDefaultAsync(x => x.ShowID == model.ShowID);
            if (show == null)
            {
                return new NotFoundObjectResult(new { status = false, message = "Сеанс не найден" });
            }

            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(model.PromoCode))
            {
                var promo = await _context.Promos.FirstOrDefaultAsync(p =>
                    p.PromoCode == model.PromoCode &&
                    p.IsActive &&
                    p.ValidFrom <= DateTime.UtcNow &&
                    p.ValidTo >= DateTime.UtcNow);
                if (promo != null) discount = promo.DiscountPercent / 100m;
            }

            var existing = await _context.Tickets
                .Where(t => t.ShowID == model.ShowID && t.Status != "возврат")
                .Select(t => new { t.RowNumber, t.SeatNumber })
                .ToListAsync();

            var created = new List<Ticket>();
            foreach (var s in model.Seats)
            {
                if (s.RowNumber < 1 || s.RowNumber > show.Hall.RowsCount ||
                    s.SeatNumber < 1 || s.SeatNumber > show.Hall.SeatsPerRow)
                {
                    return new BadRequestObjectResult(new { status = false, message = $"Неверное место {s.RowNumber}-{s.SeatNumber}" });
                }
                if (existing.Any(e => e.RowNumber == s.RowNumber && e.SeatNumber == s.SeatNumber))
                {
                    return new ConflictObjectResult(new { status = false, message = $"Место {s.RowNumber}-{s.SeatNumber} уже занято" });
                }

                var price = show.BasePrice * (1 - discount);
                var t = new Ticket
                {
                    ShowID = model.ShowID,
                    UserID = userID,
                    RowNumber = s.RowNumber,
                    SeatNumber = s.SeatNumber,
                    Price = price,
                    Status = "оплачен",
                    CreatedAt = DateTime.UtcNow,
                    QrCode = "tmp"
                };
                created.Add(t);
            }

            await _context.Tickets.AddRangeAsync(created);
            await _context.SaveChangesAsync();

            foreach (var t in created)
            {
                t.QrCode = _qr.Make(t.TicketID);
            }
            await _context.SaveChangesAsync();

            foreach (var t in created)
            {
                await _hub.Clients.All.SendAsync("SeatTaken", t.ShowID, t.RowNumber, t.SeatNumber);
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Билеты оплачены",
                tickets = created.Select(t => new
                {
                    t.TicketID,
                    t.ShowID,
                    t.RowNumber,
                    t.SeatNumber,
                    t.Price,
                    t.QrCode,
                    t.Status
                })
            });
        }

        public async Task<IActionResult> GetMyTickets(int userID)
        {
            var list = await _context.Tickets
                .Include(x => x.Show).ThenInclude(s => s.Movie)
                .Include(x => x.Show).ThenInclude(s => s.Hall)
                .Where(x => x.UserID == userID)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.TicketID,
                    x.RowNumber,
                    x.SeatNumber,
                    x.Price,
                    x.QrCode,
                    x.Status,
                    x.IsCheckedIn,
                    x.CheckedInAt,
                    Show = new
                    {
                        x.Show.ShowID,
                        x.Show.StartTime,
                        MovieTitle = x.Show.Movie.Title,
                        HallName = x.Show.Hall.HallName,
                        Poster = x.Show.Movie.PosterUrl
                    }
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, tickets = list });
        }

        public async Task<IActionResult> CreateBarOrder(int userID, BarOrderModel model)
        {
            if (model.Lines == null || model.Lines.Count == 0)
            {
                return new BadRequestObjectResult(new { status = false, message = "Корзина пустая" });
            }

            var ids = model.Lines.Select(x => x.BarItemID).ToList();
            var items = await _context.BarItems.Where(x => ids.Contains(x.BarItemID)).ToListAsync();

            decimal total = 0;
            var order = new BarOrder
            {
                UserID = userID,
                ShowID = model.ShowID,
                Status = "оплачен",
                CreatedAt = DateTime.UtcNow
            };

            foreach (var line in model.Lines)
            {
                var item = items.FirstOrDefault(i => i.BarItemID == line.BarItemID);
                if (item == null) continue;
                if (item.StockCount < line.Count)
                {
                    return new BadRequestObjectResult(new { status = false, message = $"Недостаточно товара: {item.ItemName}" });
                }
                item.StockCount -= line.Count;
                var lineSum = item.Price * line.Count;
                total += lineSum;
                order.Items.Add(new BarOrderItem
                {
                    BarItemID = item.BarItemID,
                    Count = line.Count,
                    Price = lineSum
                });
            }

            order.TotalSum = total;
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Заказ оформлен",
                orderID = order.BarOrderID,
                total = order.TotalSum
            });
        }

        public async Task<IActionResult> GetMyBarOrders(int userID)
        {
            var list = await _context.BarOrders
                .Include(x => x.Items).ThenInclude(i => i.BarItem)
                .Where(x => x.UserID == userID)
                .OrderByDescending(x => x.CreatedAt)
                .Select(o => new
                {
                    o.BarOrderID,
                    o.TotalSum,
                    o.Status,
                    o.CreatedAt,
                    Items = o.Items.Select(i => new { i.BarItemID, ItemName = i.BarItem.ItemName, i.Count, i.Price })
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, orders = list });
        }
    }
}
