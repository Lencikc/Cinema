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
    public class CashierService : ICashierService
    {
        private readonly ContextDb _context;
        private readonly QrCodeMaker _qr;
        private readonly IHubContext<CinemaHub> _hub;

        public CashierService(ContextDb context, QrCodeMaker qr, IHubContext<CinemaHub> hub)
        {
            _context = context;
            _qr = qr;
            _hub = hub;
        }

        public async Task<IActionResult> SellTickets(BuyTicketModel model)
        {
            var show = await _context.Shows.Include(x => x.Hall).FirstOrDefaultAsync(x => x.ShowID == model.ShowID);
            if (show == null)
                return new NotFoundObjectResult(new { status = false, message = "Сеанс не найден" });

            if (model.Seats == null || model.Seats.Count == 0)
                return new BadRequestObjectResult(new { status = false, message = "Места не выбраны" });

            var existing = await _context.Tickets
                .Where(t => t.ShowID == model.ShowID && t.Status != "возврат")
                .Select(t => new { t.RowNumber, t.SeatNumber })
                .ToListAsync();

            var created = new List<Ticket>();
            foreach (var s in model.Seats)
            {
                if (existing.Any(e => e.RowNumber == s.RowNumber && e.SeatNumber == s.SeatNumber))
                {
                    return new ConflictObjectResult(new { status = false, message = $"Место {s.RowNumber}-{s.SeatNumber} уже занято" });
                }

                created.Add(new Ticket
                {
                    ShowID = model.ShowID,
                    UserID = null,
                    RowNumber = s.RowNumber,
                    SeatNumber = s.SeatNumber,
                    Price = show.BasePrice,
                    Status = "касса",
                    CreatedAt = DateTime.UtcNow,
                    QrCode = "tmp"
                });
            }

            await _context.Tickets.AddRangeAsync(created);
            await _context.SaveChangesAsync();

            foreach (var t in created)
                t.QrCode = _qr.Make(t.TicketID);
            await _context.SaveChangesAsync();

            foreach (var t in created)
                await _hub.Clients.All.SendAsync("SeatTaken", t.ShowID, t.RowNumber, t.SeatNumber);

            return new OkObjectResult(new
            {
                status = true,
                message = "Билеты пробиты",
                total = created.Sum(x => x.Price),
                tickets = created.Select(t => new { t.TicketID, t.RowNumber, t.SeatNumber, t.Price, t.QrCode })
            });
        }

        public async Task<IActionResult> SellBar(BarOrderModel model)
        {
            if (model.Lines == null || model.Lines.Count == 0)
                return new BadRequestObjectResult(new { status = false, message = "Корзина пустая" });

            var ids = model.Lines.Select(x => x.BarItemID).ToList();
            var items = await _context.BarItems.Where(x => ids.Contains(x.BarItemID)).ToListAsync();

            decimal total = 0m;
            var order = new BarOrder
            {
                UserID = null,
                ShowID = model.ShowID,
                Status = "касса",
                CreatedAt = DateTime.UtcNow
            };

            foreach (var line in model.Lines)
            {
                var item = items.FirstOrDefault(i => i.BarItemID == line.BarItemID);
                if (item == null) continue;
                if (item.StockCount < line.Count)
                    return new BadRequestObjectResult(new { status = false, message = $"Не хватает: {item.ItemName}" });

                item.StockCount -= line.Count;
                var sum = item.Price * line.Count;
                total += sum;
                order.Items.Add(new BarOrderItem
                {
                    BarItemID = item.BarItemID,
                    Count = line.Count,
                    Price = sum
                });
            }

            order.TotalSum = total;
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Чек пробит",
                orderID = order.BarOrderID,
                total = order.TotalSum
            });
        }

        public async Task<IActionResult> RefundTicket(int ticketID)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketID == ticketID);
            return await RefundCore(ticket);
        }

        public async Task<IActionResult> RefundTicketByQr(string qrCode)
        {
            if (string.IsNullOrWhiteSpace(qrCode))
                return new BadRequestObjectResult(new { status = false, message = "Введите QR-код" });
            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.QrCode == qrCode);
            return await RefundCore(ticket);
        }

        private async Task<IActionResult> RefundCore(Models.Ticket? ticket)
        {
            if (ticket == null)
                return new NotFoundObjectResult(new { status = false, message = "Билет не найден" });
            if (ticket.Status == "возврат")
                return new BadRequestObjectResult(new { status = false, message = "Билет уже возвращён" });
            if (ticket.IsCheckedIn)
                return new BadRequestObjectResult(new { status = false, message = "Возврат невозможен — билет уже использован" });

            ticket.Status = "возврат";
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Возврат оформлен", ticketID = ticket.TicketID });
        }

        public async Task<IActionResult> GetCashShift()
        {
            var startOfDay = DateTime.UtcNow.Date;
            var ticketsSum = await _context.Tickets
                .Where(t => t.CreatedAt >= startOfDay && t.Status != "возврат")
                .SumAsync(t => (decimal?)t.Price) ?? 0;
            var refundsSum = await _context.Tickets
                .Where(t => t.CreatedAt >= startOfDay && t.Status == "возврат")
                .SumAsync(t => (decimal?)t.Price) ?? 0;
            var barSum = await _context.BarOrders
                .Where(o => o.CreatedAt >= startOfDay)
                .SumAsync(o => (decimal?)o.TotalSum) ?? 0;
            var ticketsCount = await _context.Tickets.CountAsync(t => t.CreatedAt >= startOfDay && t.Status != "возврат");

            return new OkObjectResult(new
            {
                status = true,
                shift = new
                {
                    date = startOfDay,
                    ticketsCount,
                    ticketsSum,
                    refundsSum,
                    barSum,
                    total = ticketsSum + barSum
                }
            });
        }
    }
}
