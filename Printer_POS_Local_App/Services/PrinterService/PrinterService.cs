using ESC_POS_USB_NET.Printer;
using ITravel_App.Models;
using System.Drawing;

//using QuestPDF.Fluent;
using System.Drawing.Printing;
//using System.Management;


namespace ITravel_App.Services.PrinterService
{
    public class PrinterService : IPrinterService
    {
        private readonly IWebHostEnvironment _env;

        public PrinterService(IWebHostEnvironment env)
        {
            _env = env;
        }

        //public async Task PrintBookingTicketAsync(
        //    BookingTicketDto ticket)
        //{
        //    var document =
        //        new BookingTicketDocument(
        //            ticket,
        //            Path.Combine(
        //                _env.WebRootPath,
        //                "images",
        //                "printerlogo.jpeg"));

        //    var pdfBytes = document.GeneratePdf();

        //    string tempFile =
        //        Path.Combine(
        //            Path.GetTempPath(),
        //            $"ticket_{Guid.NewGuid()}.pdf");

        //    await File.WriteAllBytesAsync(
        //        tempFile,
        //        pdfBytes);

        //    PrintPdf(tempFile);

        //    File.Delete(tempFile);
        //}

        private void PrintPdf(string pdfPath)
        {
            //using var document =
            //    PdfiumViewer.PdfDocument.Load(pdfPath);

            //using var printDocument =
            //    document.CreatePrintDocument();

            //printDocument.PrinterSettings.PrinterName =
            //    "XP-Q807K";

            //printDocument.PrintController =
            //    new StandardPrintController();

            //printDocument.Print();
        }
        //private string? FindAvailablePrinter()
        //{
        //    return PrinterSettings.InstalledPrinters
        //        .Cast<string>()
        //        .FirstOrDefault(name =>
        //            name.Contains("XP", StringComparison.OrdinalIgnoreCase) &&
        //            IsPrinterOnline(name));
        //}

        //private bool IsPrinterOnline(string printerName)
        //{
        //    try
        //    {
        //        using var searcher = new ManagementObjectSearcher(
        //            $"SELECT Name, PrinterStatus, WorkOffline FROM Win32_Printer " +
        //            $"WHERE Name = '{printerName.Replace("'", "''")}'");

        //        using var results = searcher.Get();

        //        foreach (ManagementObject printer in results)
        //        {
        //            bool workOffline =
        //                printer["WorkOffline"] != null &&
        //                Convert.ToBoolean(printer["WorkOffline"]);

        //            int printerStatus =
        //                printer["PrinterStatus"] != null
        //                    ? Convert.ToInt32(printer["PrinterStatus"])
        //                    : 0;

        //            // WorkOffline = true means printer is offline
        //            if (workOffline)
        //                return false;

        //            // PrinterStatus:
        //            // 3 = Idle
        //            // 4 = Printing
        //            // 5 = Warmup
        //            // 6 = Stopped printing
        //            // 7 = Offline
        //            if (printerStatus == 7)
        //                return false;

        //            return true;
        //        }

        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        //create ticket as image instead PDF to be faster 
        public async Task<bool> PrintBookingAsync(
           BookingTicketDto ticket)
        {
            string fallbackPrinter = "XP-Q807K";
            string logoPath =
                Path.Combine(
                    _env.WebRootPath,
                    "images",
                    "printerlogo.jpeg");

            var generator =
                new TicketImageGenerator();

            byte[] imageBytes =
                generator.Generate(
                    ticket,
                    logoPath);

            string tempImage =
                Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}.png");

            await File.WriteAllBytesAsync(
                tempImage,
                imageBytes);

            //  var printer = new Printer("XP-80C");
            //new Printer("XP-Q807K");

            //printer name changed from device to another so i use contain fn
    //        var printerName =
    //PrinterSettings
    //    .InstalledPrinters
    //    .Cast<string>()
    //    .FirstOrDefault(x =>
    //        x.Contains("XP",
    //            StringComparison.OrdinalIgnoreCase));


            var printerName = PrinterSettings
    .InstalledPrinters
    .Cast<string>()
    .FirstOrDefault(x =>
        x.Contains("XP", StringComparison.OrdinalIgnoreCase) ||
        x.Contains("POS", StringComparison.OrdinalIgnoreCase));

            // Check if printer exists

            //if (string.IsNullOrEmpty(printerName))
            //{
            //    printerName = fallbackPrinter;
            //}
            //string? printerName = FindAvailablePrinter();

            //// Printer not found or not connected
            //if (string.IsNullOrEmpty(printerName))
            //{
            //    return false;
            //}
            // Convert PNG bytes to Bitmap
            //using var ms = new MemoryStream(imageBytes);
            //using var bitmap = new Bitmap(ms);
            var printer = new Printer(printerName);
            printer.InitializePrint();

            printer.Image(tempImage);
            // IMPORTANT: send Bitmap, not PNG filename
           // printer.Image(bitmap);

            printer.Append("\n");
            printer.Append("\n");

            printer.FullPaperCut();

            printer.PrintDocument();

            File.Delete(tempImage);
            return true;
        }
    }
}
