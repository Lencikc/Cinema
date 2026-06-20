using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Interfaces
{
    public interface IControllerService
    {
        Task<IActionResult> CheckTicket(CheckTicketModel model);
        Task<IActionResult> GetTodayShows();
        Task<IActionResult> GetAttendance(int showID);
    }
}
