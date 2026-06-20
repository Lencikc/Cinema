namespace CinemaAPI.UniversalMethods
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string inputed, string hashed)
        {
            return BCrypt.Net.BCrypt.Verify(inputed, hashed);
        }
    }
}
