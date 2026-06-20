using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Interfaces
{
    public interface IAdminService
    {
        Task<IActionResult> GetUsers();
        Task<IActionResult> EditUser(EditUserModel model);
        Task<IActionResult> DeleteUser(int userID);

        Task<IActionResult> AddMovie(AddMovieModel model);
        Task<IActionResult> EditMovie(EditMovieModel model);
        Task<IActionResult> DeleteMovie(int movieID);

        Task<IActionResult> AddHall(AddHallModel model);
        Task<IActionResult> GetHalls();
        Task<IActionResult> DeleteHall(int hallID);

        Task<IActionResult> AddShow(AddShowModel model);
        Task<IActionResult> DeleteShow(int showID);

        Task<IActionResult> AddBarItem(AddBarItemModel model);
        Task<IActionResult> EditBarItem(int barItemID, AddBarItemModel model);
        Task<IActionResult> DeleteBarItem(int barItemID);

        Task<IActionResult> AddPromo(AddPromoModel model);
        Task<IActionResult> GetPromos();
        Task<IActionResult> DeletePromo(int promoID);

        Task<IActionResult> GetDashboard();
        Task<IActionResult> GetSalesReport();
    }
}
