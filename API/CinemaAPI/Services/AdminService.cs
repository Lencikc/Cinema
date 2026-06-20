using CinemaAPI.Connection;
using CinemaAPI.Interfaces;
using CinemaAPI.Models;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly ContextDb _context;

        public AdminService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetUsers()
        {
            var list = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Login)
                .OrderBy(x => x.LastName)
                .Select(x => new
                {
                    x.UserID,
                    x.LastName,
                    x.FirstName,
                    x.Patronymic,
                    x.Phone,
                    x.RoleID,
                    RoleName = x.Role.RoleName,
                    Email = x.Login.Email
                })
                .ToListAsync();
            return new OkObjectResult(new { status = true, users = list });
        }

        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.UserID == model.UserID);
            if (u == null) return new NotFoundObjectResult(new { status = false, message = "Пользователь не найден" });

            u.LastName = model.LastName;
            u.FirstName = model.FirstName;
            u.Patronymic = model.Patronymic;
            u.Phone = model.Phone;
            u.RoleID = model.RoleID;

            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Сохранено" });
        }

        public async Task<IActionResult> DeleteUser(int userID)
        {
            var u = await _context.Users.Include(x => x.Login).FirstOrDefaultAsync(x => x.UserID == userID);
            if (u == null) return new NotFoundObjectResult(new { status = false, message = "Не найден" });

            var sessions = await _context.Sessions.Where(s => s.UserID == userID).ToListAsync();
            _context.Sessions.RemoveRange(sessions);

            var tickets = await _context.Tickets.Where(t => t.UserID == userID).ToListAsync();
            foreach (var t in tickets) t.UserID = null;

            var orders = await _context.BarOrders.Where(o => o.UserID == userID).ToListAsync();
            foreach (var o in orders) o.UserID = null;

            var login = u.Login;
            _context.Users.Remove(u);
            if (login != null) _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { status = true, message = "Удалено" });
        }

        public async Task<IActionResult> AddMovie(AddMovieModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                return new BadRequestObjectResult(new { status = false, message = "Введите название" });
            if (string.IsNullOrWhiteSpace(model.Description))
                return new BadRequestObjectResult(new { status = false, message = "Введите описание" });
            if (model.GenreID <= 0 || !await _context.Genres.AnyAsync(g => g.GenreID == model.GenreID))
                return new BadRequestObjectResult(new { status = false, message = "Выберите жанр" });
            if (model.AgeRatingID <= 0 || !await _context.AgeRatings.AnyAsync(a => a.AgeRatingID == model.AgeRatingID))
                return new BadRequestObjectResult(new { status = false, message = "Выберите возрастной рейтинг" });
            if (model.DurationMinutes <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Длительность должна быть больше 0" });
            if (model.ReleaseYear < 1900 || model.ReleaseYear > DateTime.UtcNow.Year + 2)
                return new BadRequestObjectResult(new { status = false, message = "Некорректный год" });

            var m = new Movie
            {
                Title = model.Title,
                Description = model.Description ?? "",
                GenreID = model.GenreID,
                DurationMinutes = model.DurationMinutes,
                AgeRatingID = model.AgeRatingID,
                PosterUrl = model.PosterUrl,
                ReleaseYear = model.ReleaseYear,
                IsActive = true
            };
            await _context.AddAsync(m);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Фильм добавлен", movieID = m.MovieID });
        }

        public async Task<IActionResult> EditMovie(EditMovieModel model)
        {
            var m = await _context.Movies.FirstOrDefaultAsync(x => x.MovieID == model.MovieID);
            if (m == null) return new NotFoundObjectResult(new { status = false, message = "Фильм не найден" });

            if (string.IsNullOrWhiteSpace(model.Title))
                return new BadRequestObjectResult(new { status = false, message = "Введите название" });
            if (string.IsNullOrWhiteSpace(model.Description))
                return new BadRequestObjectResult(new { status = false, message = "Введите описание" });
            if (model.GenreID <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Выберите жанр" });
            if (model.AgeRatingID <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Выберите возрастной рейтинг" });
            if (model.DurationMinutes <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Длительность должна быть больше 0" });

            m.Title = model.Title;
            m.Description = model.Description;
            m.GenreID = model.GenreID;
            m.DurationMinutes = model.DurationMinutes;
            m.AgeRatingID = model.AgeRatingID;
            m.PosterUrl = model.PosterUrl;
            m.ReleaseYear = model.ReleaseYear;
            m.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Изменения сохранены" });
        }

        public async Task<IActionResult> DeleteMovie(int movieID)
        {
            var m = await _context.Movies.FirstOrDefaultAsync(x => x.MovieID == movieID);
            if (m == null) return new NotFoundObjectResult(new { status = false, message = "Не найден" });

            _context.Movies.Remove(m);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Удалено" });
        }

        public async Task<IActionResult> AddHall(AddHallModel model)
        {
            if (string.IsNullOrWhiteSpace(model.HallName))
                return new BadRequestObjectResult(new { status = false, message = "Введите название" });
            if (string.IsNullOrWhiteSpace(model.HallType))
                return new BadRequestObjectResult(new { status = false, message = "Введите тип зала" });
            if (model.RowsCount <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Количество рядов должно быть больше 0" });
            if (model.SeatsPerRow <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Количество мест в ряду должно быть больше 0" });

            var h = new Hall
            {
                HallName = model.HallName,
                RowsCount = model.RowsCount,
                SeatsPerRow = model.SeatsPerRow,
                HallType = model.HallType ?? "Standard"
            };
            await _context.AddAsync(h);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Зал добавлен", hallID = h.HallID });
        }

        public async Task<IActionResult> GetHalls()
        {
            var list = await _context.Halls.OrderBy(x => x.HallName).ToListAsync();
            return new OkObjectResult(new { status = true, halls = list });
        }

        public async Task<IActionResult> DeleteHall(int hallID)
        {
            var h = await _context.Halls.FirstOrDefaultAsync(x => x.HallID == hallID);
            if (h == null) return new NotFoundObjectResult(new { status = false, message = "Не найден" });

            _context.Halls.Remove(h);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Удалено" });
        }

        public async Task<IActionResult> AddShow(AddShowModel model)
        {
            if (model.StartTime <= DateTime.UtcNow)
                return new BadRequestObjectResult(new { status = false, message = "Дата сеанса не может быть в прошлом" });
            if (model.BasePrice <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Цена должна быть положительной" });

            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.MovieID == model.MovieID);
            if (movie == null) return new BadRequestObjectResult(new { status = false, message = "Фильм не найден" });

            var hall = await _context.Halls.FirstOrDefaultAsync(x => x.HallID == model.HallID);
            if (hall == null) return new BadRequestObjectResult(new { status = false, message = "Зал не найден" });

            var endTime = model.StartTime.AddMinutes(movie.DurationMinutes + 15);
            var overlap = await _context.Shows.AnyAsync(s =>
                s.HallID == model.HallID &&
                s.StartTime < endTime &&
                s.StartTime.AddMinutes(s.Movie.DurationMinutes + 15) > model.StartTime);

            if (overlap)
                return new ConflictObjectResult(new { status = false, message = "Пересечение по времени с другим сеансом" });

            var show = new Show
            {
                MovieID = model.MovieID,
                HallID = model.HallID,
                StartTime = model.StartTime,
                BasePrice = model.BasePrice,
                Status = "запланирован"
            };
            await _context.AddAsync(show);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { status = true, message = "Сеанс создан", showID = show.ShowID });
        }

        public async Task<IActionResult> DeleteShow(int showID)
        {
            var s = await _context.Shows.FirstOrDefaultAsync(x => x.ShowID == showID);
            if (s == null) return new NotFoundObjectResult(new { status = false, message = "Не найден" });

            _context.Shows.Remove(s);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Сеанс удалён" });
        }

        public async Task<IActionResult> AddBarItem(AddBarItemModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ItemName))
                return new BadRequestObjectResult(new { status = false, message = "Введите название" });
            if (model.BarCategoryID <= 0 || !await _context.BarCategories.AnyAsync(c => c.BarCategoryID == model.BarCategoryID))
                return new BadRequestObjectResult(new { status = false, message = "Выберите категорию" });
            if (model.Price <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Цена должна быть положительной" });
            if (model.StockCount < 0)
                return new BadRequestObjectResult(new { status = false, message = "Остаток не может быть отрицательным" });

            var b = new BarItem
            {
                ItemName = model.ItemName,
                ItemDescription = model.ItemDescription,
                BarCategoryID = model.BarCategoryID,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                StockCount = model.StockCount,
                IsActive = true
            };
            await _context.AddAsync(b);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Товар добавлен" });
        }

        public async Task<IActionResult> EditBarItem(int barItemID, AddBarItemModel model)
        {
            var b = await _context.BarItems.FirstOrDefaultAsync(x => x.BarItemID == barItemID);
            if (b == null) return new NotFoundObjectResult(new { status = false, message = "Не найдено" });
            if (string.IsNullOrWhiteSpace(model.ItemName))
                return new BadRequestObjectResult(new { status = false, message = "Введите название" });
            if (model.BarCategoryID <= 0)
                return new BadRequestObjectResult(new { status = false, message = "Выберите категорию" });

            b.ItemName = model.ItemName;
            b.ItemDescription = model.ItemDescription;
            b.BarCategoryID = model.BarCategoryID;
            b.Price = model.Price;
            b.ImageUrl = model.ImageUrl;
            b.StockCount = model.StockCount;

            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Сохранено" });
        }

        public async Task<IActionResult> DeleteBarItem(int barItemID)
        {
            var b = await _context.BarItems.FirstOrDefaultAsync(x => x.BarItemID == barItemID);
            if (b == null) return new NotFoundObjectResult(new { status = false, message = "Не найдено" });

            _context.BarItems.Remove(b);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Удалено" });
        }

        public async Task<IActionResult> AddPromo(AddPromoModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PromoCode))
                return new BadRequestObjectResult(new { status = false, message = "Введите код промокода" });
            if (string.IsNullOrWhiteSpace(model.PromoName))
                return new BadRequestObjectResult(new { status = false, message = "Введите название" });
            if (model.DiscountPercent <= 0 || model.DiscountPercent > 100)
                return new BadRequestObjectResult(new { status = false, message = "Скидка от 1 до 100%" });

            var fromUtc = DateTime.SpecifyKind(model.ValidFrom, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(model.ValidTo, DateTimeKind.Utc);
            var todayUtc = DateTime.UtcNow.Date;
            if (fromUtc.Date < todayUtc)
                return new BadRequestObjectResult(new { status = false, message = "Дата начала не может быть раньше сегодня" });
            if (toUtc.Date < fromUtc.Date)
                return new BadRequestObjectResult(new { status = false, message = "Дата окончания не может быть раньше даты начала" });

            var p = new Promo
            {
                PromoCode = model.PromoCode,
                PromoName = model.PromoName,
                DiscountPercent = model.DiscountPercent,
                ValidFrom = fromUtc,
                ValidTo = toUtc,
                IsActive = true
            };
            await _context.AddAsync(p);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Промокод создан" });
        }

        public async Task<IActionResult> GetPromos()
        {
            var list = await _context.Promos.OrderByDescending(x => x.ValidTo).ToListAsync();
            return new OkObjectResult(new { status = true, promos = list });
        }

        public async Task<IActionResult> DeletePromo(int promoID)
        {
            var p = await _context.Promos.FirstOrDefaultAsync(x => x.PromoID == promoID);
            if (p == null) return new NotFoundObjectResult(new { status = false, message = "Не найден" });
            _context.Promos.Remove(p);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Удалён" });
        }

        public async Task<IActionResult> GetDashboard()
        {
            var startOfDay = DateTime.UtcNow.AddHours(-12).Date;
            var endOfDay = startOfDay.AddDays(2);

            var ticketsToday = await _context.Tickets.CountAsync(t => t.CreatedAt >= startOfDay && t.Status != "возврат");
            var revenueToday = await _context.Tickets
                .Where(t => t.CreatedAt >= startOfDay && t.Status != "возврат")
                .SumAsync(t => (decimal?)t.Price) ?? 0;
            var barRevenueToday = await _context.BarOrders
                .Where(o => o.CreatedAt >= startOfDay)
                .SumAsync(o => (decimal?)o.TotalSum) ?? 0;
            var activeShows = await _context.Shows.CountAsync(x => x.StartTime >= startOfDay && x.StartTime < endOfDay);
            var totalUsers = await _context.Users.CountAsync(x => x.RoleID == 3);

            var topMovies = await _context.Tickets
                .Where(t => t.Status != "возврат")
                .GroupBy(t => t.Show.Movie.Title)
                .Select(g => new { title = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(5)
                .ToListAsync();

            var hallLoad = await _context.Shows
                .Where(s => s.StartTime >= startOfDay && s.StartTime < endOfDay)
                .Select(s => new
                {
                    s.ShowID,
                    movie = s.Movie.Title,
                    hall = s.Hall.HallName,
                    s.StartTime,
                    capacity = s.Hall.RowsCount * s.Hall.SeatsPerRow,
                    sold = _context.Tickets.Count(t => t.ShowID == s.ShowID && t.Status != "возврат")
                })
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            return new OkObjectResult(new
            {
                status = true,
                dashboard = new
                {
                    ticketsToday,
                    revenueToday,
                    barRevenueToday,
                    totalToday = revenueToday + barRevenueToday,
                    activeShows,
                    totalUsers,
                    topMovies,
                    hallLoad
                }
            });
        }

        public async Task<IActionResult> GetSalesReport()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var byDay = await _context.Tickets
                .Where(t => t.CreatedAt >= startOfMonth && t.Status != "возврат")
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { day = g.Key, sum = g.Sum(t => t.Price), count = g.Count() })
                .OrderBy(x => x.day)
                .ToListAsync();

            var totalSold = await _context.Tickets.CountAsync(x => x.Status != "возврат");
            var totalRefund = await _context.Tickets.CountAsync(x => x.Status == "возврат");
            var revenueAll = await _context.Tickets.Where(x => x.Status != "возврат").SumAsync(x => (decimal?)x.Price) ?? 0;

            return new OkObjectResult(new
            {
                status = true,
                report = new
                {
                    byDay,
                    totals = new { totalSold, totalRefund, revenueAll }
                }
            });
        }
    }
}
