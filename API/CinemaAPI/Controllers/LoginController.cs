using CinemaAPI.CustomAttributes;
using CinemaAPI.Interfaces;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        [Route("/cinema/signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            return await _loginService.SignUp(model);
        }

        [HttpPost]
        [Route("/cinema/signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            return await _loginService.SignIn(model);
        }

        [HttpGet]
        [Route("/cinema/profile/{userID}")]
        [RoleAuthorize([1, 2, 3, 4])]
        public async Task<IActionResult> GetProfile(int userID)
        {
            return await _loginService.GetProfile(userID);
        }
    }
}
