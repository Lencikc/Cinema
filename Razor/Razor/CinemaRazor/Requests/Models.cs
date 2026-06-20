namespace CinemaRazor.Requests
{
    public class SignInModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class SignUpModel
    {
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class EditUserModel
    {
        public int UserID { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public int RoleID { get; set; }
    }

    public class AddMovieModel
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int GenreID { get; set; }
        public int DurationMinutes { get; set; } = 90;
        public int AgeRatingID { get; set; }
        public string? PosterUrl { get; set; }
        public int ReleaseYear { get; set; } = DateTime.Today.Year;
    }

    public class EditMovieModel : AddMovieModel
    {
        public int MovieID { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AddHallModel
    {
        public string HallName { get; set; } = "";
        public int RowsCount { get; set; } = 8;
        public int SeatsPerRow { get; set; } = 12;
        public string HallType { get; set; } = "Standard";
    }

    public class AddShowModel
    {
        public int MovieID { get; set; }
        public int HallID { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now.AddDays(1);
        public decimal BasePrice { get; set; } = 350;
    }

    public class SeatPick
    {
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
    }

    public class BuyTicketModel
    {
        public int ShowID { get; set; }
        public List<SeatPick> Seats { get; set; } = new();
        public string? PromoCode { get; set; }
    }

    public class AddBarItemModel
    {
        public string ItemName { get; set; } = "";
        public string? ItemDescription { get; set; }
        public int BarCategoryID { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockCount { get; set; }
    }

    public class GenreDto
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; } = "";
    }
    public class AgeRatingDto
    {
        public int AgeRatingID { get; set; }
        public int MinAge { get; set; }
        public string Label { get; set; } = "";
    }
    public class BarCategoryDto
    {
        public int BarCategoryID { get; set; }
        public string CategoryName { get; set; } = "";
    }

    public class BarOrderLine
    {
        public int BarItemID { get; set; }
        public int Count { get; set; }
    }

    public class BarOrderModel
    {
        public int? ShowID { get; set; }
        public List<BarOrderLine> Lines { get; set; } = new();
    }

    public class AddPromoModel
    {
        public string PromoCode { get; set; } = "";
        public string PromoName { get; set; } = "";
        public int DiscountPercent { get; set; } = 10;
        public DateTime ValidFrom { get; set; } = DateTime.Today;
        public DateTime ValidTo { get; set; } = DateTime.Today.AddMonths(1);
    }

    public class CheckTicketModel
    {
        public string QrCode { get; set; } = "";
    }
}
