using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> BuyTickets(int userID, BuyTicketModel model);
        Task<IActionResult> GetMyTickets(int userID);
        Task<IActionResult> CreateBarOrder(int userID, BarOrderModel model);
        Task<IActionResult> GetMyBarOrders(int userID);
    }
}
