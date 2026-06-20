using CinemaAPI.CustomAttributes;
using CinemaAPI.Interfaces;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class ScannerController : ControllerBase
    {
        private readonly IControllerService _service;

        public ScannerController(IControllerService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("/cinema/scan")]
        [RoleAuthorize([1, 4])]
        public async Task<IActionResult> Scan([FromBody] CheckTicketModel model) =>
            await _service.CheckTicket(model);

        [HttpGet]
        [Route("/cinema/today")]
        [RoleAuthorize([1, 2, 4])]
        public async Task<IActionResult> Today() => await _service.GetTodayShows();

        [HttpGet]
        [Route("/cinema/attendance/{showID}")]
        [RoleAuthorize([1, 4])]
        public async Task<IActionResult> Attendance(int showID) => await _service.GetAttendance(showID);
    }
}
