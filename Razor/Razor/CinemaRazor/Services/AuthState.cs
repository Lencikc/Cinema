using CinemaRazor.Models;

namespace CinemaRazor.Services
{
    public class AuthState
    {
        public UserDto? Current { get; private set; }
        public string? Token { get; private set; }
        public bool IsLogged => Current != null;
        public bool IsAdmin => Current?.RoleID == 1;
        public bool IsCashier => Current?.RoleID == 2;
        public bool IsViewer => Current?.RoleID == 3;
        public bool IsScanner => Current?.RoleID == 4;

        public event Action? Changed;

        public void Set(UserDto user, string token)
        {
            Current = user;
            Token = token;
            Changed?.Invoke();
        }

        public void Clear()
        {
            Current = null;
            Token = null;
            Changed?.Invoke();
        }
    }
}
