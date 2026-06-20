using CinemaAPI.CustomAttributes;
using CinemaAPI.Interfaces;
using CinemaAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    public class CashierController : ControllerBase
    {
        private readonly ICashierService _cashierService;

        public CashierController(ICashierService cashierService)
        {
            _cashierService = cashierService;
        }

        [HttpPost]
        [Route("/cinema/cashier/sell")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> SellTickets([FromBody] BuyTicketModel model) =>
            await _cashierService.SellTickets(model);

        [HttpPost]
        [Route("/cinema/cashier/bar")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> SellBar([FromBody] BarOrderModel model) =>
            await _cashierService.SellBar(model);

        [HttpPut]
        [Route("/cinema/cashier/refund/{ticketID}")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> RefundTicket(int ticketID) =>
            await _cashierService.RefundTicket(ticketID);

        [HttpPost]
        [Route("/cinema/cashier/refund/qr")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> RefundTicketByQr([FromBody] CheckTicketModel model) =>
            await _cashierService.RefundTicketByQr(model.QrCode);

        [HttpGet]
        [Route("/cinema/cashier/shift")]
        [RoleAuthorize([1, 2])]
        public async Task<IActionResult> Shift() => await _cashierService.GetCashShift();
    }
}
