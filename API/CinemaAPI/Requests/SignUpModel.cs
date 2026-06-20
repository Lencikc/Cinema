namespace CinemaAPI.Requests
{
    public class SignUpModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
