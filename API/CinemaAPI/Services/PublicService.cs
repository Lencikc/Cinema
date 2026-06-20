using CinemaAPI.Connection;
using CinemaAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services
{
    public class PublicService : IPublicService
    {
        private readonly ContextDb _context;

        public PublicService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetMovies()
        {
            var list = await _context.Movies
                .Include(m => m.Genre)
                .Include(m => m.AgeRating)
                .Where(m => m.IsActive)
                .OrderBy(m => m.Title)
                .Select(m => new
                {
                    m.MovieID,
                    m.Title,
                    m.Description,
                    m.GenreID,
                    GenreName = m.Genre != null ? m.Genre.GenreName : "",
                    m.DurationMinutes,
                    m.AgeRatingID,
                    AgeRatingMin = m.AgeRating != null ? m.AgeRating.MinAge : 0,
                    AgeRatingLabel = m.AgeRating != null ? m.AgeRating.Label : "",
                    m.PosterUrl,
                    m.ReleaseYear,
                    m.IsActive
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, movies = list });
        }

        public async Task<IActionResult> GetMovie(int movieID)
        {
            var movie = await _context.Movies
                .Include(m => m.Genre)
                .Include(m => m.AgeRating)
                .Where(x => x.MovieID == movieID)
                .Select(m => new
                {
                    m.MovieID,
                    m.Title,
                    m.Description,
                    m.GenreID,
                    GenreName = m.Genre != null ? m.Genre.GenreName : "",
                    m.DurationMinutes,
                    m.AgeRatingID,
                    AgeRatingMin = m.AgeRating != null ? m.AgeRating.MinAge : 0,
                    AgeRatingLabel = m.AgeRating != null ? m.AgeRating.Label : "",
                    m.PosterUrl,
                    m.ReleaseYear,
                    m.IsActive
                })
                .FirstOrDefaultAsync();

            if (movie == null)
                return new NotFoundObjectResult(new { status = false, message = "Фильм не найден" });

            return new OkObjectResult(new { status = true, movie });
        }

        public async Task<IActionResult> GetShowsByMovie(int movieID)
        {
            var shows = await _context.Shows
                .Include(x => x.Hall)
                .Include(x => x.Movie)
                .Where(x => x.MovieID == movieID && x.StartTime > DateTime.UtcNow && x.Status != "отменён")
                .OrderBy(x => x.StartTime)
                .Select(x => new
                {
                    x.ShowID,
                    x.MovieID,
                    MovieTitle = x.Movie.Title,
                    x.HallID,
                    HallName = x.Hall.HallName,
                    HallType = x.Hall.HallType,
                    x.StartTime,
                    x.BasePrice,
                    x.Status
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, shows });
        }

        public async Task<IActionResult> GetShow(int showID)
        {
            var show = await _context.Shows
                .Include(x => x.Movie)
                .Include(x => x.Hall)
                .FirstOrDefaultAsync(x => x.ShowID == showID);

            if (show == null)
                return new NotFoundObjectResult(new { status = false, message = "Сеанс не найден" });

            return new OkObjectResult(new
            {
                status = true,
                show = new
                {
                    show.ShowID,
                    show.StartTime,
                    show.BasePrice,
                    show.Status,
                    Movie = show.Movie,
                    Hall = show.Hall
                }
            });
        }

        public async Task<IActionResult> GetSeatMap(int showID)
        {
            var show = await _context.Shows
                .Include(x => x.Hall)
                .FirstOrDefaultAsync(x => x.ShowID == showID);

            if (show == null)
                return new NotFoundObjectResult(new { status = false, message = "Сеанс не найден" });

            var taken = await _context.Tickets
                .Where(t => t.ShowID == showID && t.Status != "возврат")
                .Select(t => new { t.RowNumber, t.SeatNumber })
                .ToListAsync();

            var rows = new List<object>();
            for (int r = 1; r <= show.Hall.RowsCount; r++)
            {
                var seats = new List<object>();
                for (int s = 1; s <= show.Hall.SeatsPerRow; s++)
                {
                    bool isTaken = taken.Any(t => t.RowNumber == r && t.SeatNumber == s);
                    seats.Add(new { row = r, seat = s, taken = isTaken });
                }
                rows.Add(new { rowNumber = r, seats });
            }

            return new OkObjectResult(new
            {
                status = true,
                hall = new { show.Hall.HallName, show.Hall.RowsCount, show.Hall.SeatsPerRow },
                basePrice = show.BasePrice,
                rows
            });
        }

        public async Task<IActionResult> GetBarItems()
        {
            var items = await _context.BarItems
                .Include(x => x.BarCategory)
                .Where(x => x.IsActive)
                .OrderBy(x => x.BarCategory!.CategoryName).ThenBy(x => x.ItemName)
                .Select(x => new
                {
                    x.BarItemID,
                    x.ItemName,
                    x.ItemDescription,
                    x.BarCategoryID,
                    Category = x.BarCategory != null ? x.BarCategory.CategoryName : "",
                    x.Price,
                    x.ImageUrl,
                    x.StockCount,
                    x.IsActive
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, items });
        }
    }
}
