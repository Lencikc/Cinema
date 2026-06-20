using CinemaAPI.CustomAttributes;
using CinemaAPI.Interfaces;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("/cinema/user/{userID}/buy")]
        [RoleAuthorize([3])]
        public async Task<IActionResult> BuyTickets(int userID, [FromBody] BuyTicketModel model)
        {
            return await _userService.BuyTickets(userID, model);
        }

        [HttpGet]
        [Route("/cinema/user/{userID}/tickets")]
        [RoleAuthorize([1, 3])]
        public async Task<IActionResult> MyTickets(int userID)
        {
            return await _userService.GetMyTickets(userID);
        }

        [HttpPost]
        [Route("/cinema/user/{userID}/bar/order")]
        [RoleAuthorize([3])]
        public async Task<IActionResult> BarOrder(int userID, [FromBody] BarOrderModel model)
        {
            return await _userService.CreateBarOrder(userID, model);
        }

        [HttpGet]
        [Route("/cinema/user/{userID}/bar/orders")]
        [RoleAuthorize([1, 3])]
        public async Task<IActionResult> MyBarOrders(int userID)
        {
            return await _userService.GetMyBarOrders(userID);
        }
    }
}
