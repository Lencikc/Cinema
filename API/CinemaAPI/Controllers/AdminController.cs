using CinemaAPI.CustomAttributes;
using CinemaAPI.Interfaces;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        [Route("/cinema/admin/users")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> Users() => await _adminService.GetUsers();

        [HttpPut]
        [Route("/cinema/admin/edituser")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> EditUser([FromBody] EditUserModel model) =>
            await _adminService.EditUser(model);

        [HttpDelete]
        [Route("/cinema/admin/deleteuser/{userID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> DeleteUser(int userID) => await _adminService.DeleteUser(userID);

        [HttpPost]
        [Route("/cinema/admin/movie")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> AddMovie([FromBody] AddMovieModel model) => await _adminService.AddMovie(model);

        [HttpPut]
        [Route("/cinema/admin/movie")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> EditMovie([FromBody] EditMovieModel model) => await _adminService.EditMovie(model);

        [HttpDelete]
        [Route("/cinema/admin/movie/{movieID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> DeleteMovie(int movieID) => await _adminService.DeleteMovie(movieID);

        [HttpPost]
        [Route("/cinema/admin/hall")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> AddHall([FromBody] AddHallModel model) => await _adminService.AddHall(model);

        [HttpGet]
        [Route("/cinema/admin/halls")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> Halls() => await _adminService.GetHalls();

        [HttpDelete]
        [Route("/cinema/admin/hall/{hallID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> DeleteHall(int hallID) => await _adminService.DeleteHall(hallID);

        [HttpPost]
        [Route("/cinema/admin/show")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> AddShow([FromBody] AddShowModel model) => await _adminService.AddShow(model);

        [HttpDelete]
        [Route("/cinema/admin/show/{showID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> DeleteShow(int showID) => await _adminService.DeleteShow(showID);

        [HttpPost]
        [Route("/cinema/admin/baritem")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> AddBarItem([FromBody] AddBarItemModel model) => await _adminService.AddBarItem(model);

        [HttpPut]
        [Route("/cinema/admin/baritem/{barItemID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> EditBarItem(int barItemID, [FromBody] AddBarItemModel model) =>
            await _adminService.EditBarItem(barItemID, model);

        [HttpDelete]
        [Route("/cinema/admin/baritem/{barItemID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> DeleteBarItem(int barItemID) => await _adminService.DeleteBarItem(barItemID);

        [HttpPost]
        [Route("/cinema/admin/promo")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> AddPromo([FromBody] AddPromoModel model) => await _adminService.AddPromo(model);

        [HttpGet]
        [Route("/cinema/admin/promos")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> Promos() => await _adminService.GetPromos();

        [HttpDelete]
        [Route("/cinema/admin/promo/{promoID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> DeletePromo(int promoID) => await _adminService.DeletePromo(promoID);

        [HttpGet]
        [Route("/cinema/admin/dashboard")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> Dashboard() => await _adminService.GetDashboard();

        [HttpGet]
        [Route("/cinema/admin/sales")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> Sales() => await _adminService.GetSalesReport();
    }
}
