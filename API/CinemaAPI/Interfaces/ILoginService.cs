using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Interfaces
{
    public interface ILoginService
    {
        Task<IActionResult> SignUp(SignUpModel model);
        Task<IActionResult> SignIn(SignInModel model);
        Task<IActionResult> GetProfile(int userID);
    }
}
