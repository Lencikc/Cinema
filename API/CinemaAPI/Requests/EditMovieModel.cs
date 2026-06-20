namespace CinemaAPI.Requests
{
    public class EditMovieModel
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int GenreID { get; set; }
        public int DurationMinutes { get; set; }
        public int AgeRatingID { get; set; }
        public string? PosterUrl { get; set; }
        public int ReleaseYear { get; set; }
        public bool IsActive { get; set; }
    }
}
