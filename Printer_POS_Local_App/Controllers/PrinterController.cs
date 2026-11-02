using ITravel_App.Models;
using ITravel_App.Services.PrinterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Printer_POS_Local_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PrinterController : Controller
    {
     
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IPrinterService _printer;
        public PrinterController(IPrinterService printer , IHttpContextAccessor httpContextAccessor)
        {
           
            _httpContextAccessor = httpContextAccessor;
           
            _printer = printer;
        }

        /// <summary>
        /// Print booking ticket.
        /// </summary>
        /// <param name="ticket">Booking ticket information.</param>
        /// <returns>Success response.</returns>
        /// /// <remarks>
        /// Sample request:
        ///
        /// POS /api/PrintTicket
        ///
        /// </remarks>
        /// <response code="200">Success response.</response>
        [HttpPost("PrintTicket")]
        public async Task<IActionResult> Print(
       BookingTicketDto ticket)
        {
           // return ());


            return Ok(new
            {
                success = await _printer.PrintBookingAsync(ticket)
            });
        }
    }
}
