using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Interfaces
{
    public interface IPublicService
    {
        Task<IActionResult> GetMovies();
        Task<IActionResult> GetMovie(int movieID);
        Task<IActionResult> GetShowsByMovie(int movieID);
        Task<IActionResult> GetShow(int showID);
        Task<IActionResult> GetSeatMap(int showID);
        Task<IActionResult> GetBarItems();
    }
}
