using ITravel_App.Models;

namespace ITravel_App.Services.PrinterService
{
    public interface IPrinterService
    {
     //   Task PrintBookingTicketAsync(
     //BookingTicketDto ticket);

        Task<bool> PrintBookingAsync(BookingTicketDto ticket);
    }
}
