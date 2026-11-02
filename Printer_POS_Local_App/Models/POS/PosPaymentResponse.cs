namespace Printer_POS_Local_App.Models.POS
{
    public class PosPaymentResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = "";

        public string RawResponse { get; set; } = "";

        public PosReceipt? Receipt { get; set; }
    }
}
