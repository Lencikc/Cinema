using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Interfaces
{
    public interface ICashierService
    {
        Task<IActionResult> SellTickets(BuyTicketModel model);
        Task<IActionResult> SellBar(BarOrderModel model);
        Task<IActionResult> RefundTicket(int ticketID);
        Task<IActionResult> RefundTicketByQr(string qrCode);
        Task<IActionResult> GetCashShift();
    }
}
