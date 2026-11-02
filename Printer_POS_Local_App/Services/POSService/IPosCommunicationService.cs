using Printer_POS_Local_App.Models.POS;

namespace Printer_POS_Local_App.Services.POSService
{
    public interface IPosCommunicationService
    {
        Task<PosPaymentResponse> ProcessPaymentAsync(
          PosPaymentRequest request);
    }
}
