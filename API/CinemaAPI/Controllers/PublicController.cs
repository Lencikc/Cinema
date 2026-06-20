using CinemaAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class PublicController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public PublicController(IPublicService publicService)
        {
            _publicService = publicService;
        }

        [HttpGet]
        [Route("/cinema/movies")]
        public async Task<IActionResult> Movies() => await _publicService.GetMovies();

        [HttpGet]
        [Route("/cinema/movies/{movieID}")]
        public async Task<IActionResult> Movie(int movieID) => await _publicService.GetMovie(movieID);

        [HttpGet]
        [Route("/cinema/movies/{movieID}/shows")]
        public async Task<IActionResult> ShowsByMovie(int movieID) => await _publicService.GetShowsByMovie(movieID);

        [HttpGet]
        [Route("/cinema/shows/{showID}")]
        public async Task<IActionResult> Show(int showID) => await _publicService.GetShow(showID);

        [HttpGet]
        [Route("/cinema/shows/{showID}/seats")]
        public async Task<IActionResult> SeatMap(int showID) => await _publicService.GetSeatMap(showID);

        [HttpGet]
        [Route("/cinema/bar")]
        public async Task<IActionResult> BarItems() => await _publicService.GetBarItems();
    }
}
