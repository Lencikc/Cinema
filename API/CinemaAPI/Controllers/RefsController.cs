using CinemaAPI.Connection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers
{
    public class RefsController : ControllerBase
    {
        private readonly ContextDb _context;

        public RefsController(ContextDb context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/cinema/refs/genres")]
        public async Task<IActionResult> Genres()
        {
            var list = await _context.Genres.OrderBy(x => x.GenreName).ToListAsync();
            return new OkObjectResult(new { status = true, genres = list });
        }

        [HttpGet]
        [Route("/cinema/refs/ageratings")]
        public async Task<IActionResult> AgeRatings()
        {
            var list = await _context.AgeRatings.OrderBy(x => x.MinAge).ToListAsync();
            return new OkObjectResult(new { status = true, ageRatings = list });
        }

        [HttpGet]
        [Route("/cinema/refs/barcategories")]
        public async Task<IActionResult> BarCategories()
        {
            var list = await _context.BarCategories.OrderBy(x => x.CategoryName).ToListAsync();
            return new OkObjectResult(new { status = true, categories = list });
        }
    }
}
