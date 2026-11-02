
using SkiaSharp;
using System.Drawing;

namespace ITravel_App.Models
{
    public class TicketImageGenerator
    {

        private const int Width = 576;
        private const int Margin = 20;
        private const int RowSpacing = 20;

        public byte[] Generate(
            BookingTicketDto ticket,
            string logoPath)
        {
            var t = TicketTranslations.Get(ticket.lang_code);
            var titlePaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 34,
                IsAntialias = true,
                FakeBoldText = true
            };

            //var textPaint = new SKPaint
            //{
            //    Color = SKColors.Black,
            //    TextSize = 26,
            //    IsAntialias = true
            //};
            var typeface = SKTypeface.FromFile("fonts/Roboto-Regular.ttf");

            var textPaint = new SKPaint
            {
                Typeface = typeface,
                TextSize = 26,
                IsAntialias = true,
                Color = SKColors.Black
            };
            // Calculate dynamic height
            int estimatedHeight =
                CalculateHeight(ticket, textPaint, logoPath);

            using var bitmap =
                new SKBitmap(Width, estimatedHeight);

            using var canvas =
                new SKCanvas(bitmap);

            canvas.Clear(SKColors.White);

            int y = 0;
             const int LogoBottomMargin = 60;
            #region Logo

            if (File.Exists(logoPath))
            {
                using var logo = SKBitmap.Decode(logoPath);

                // float maxLogoWidth = Width * 0.7f; // 70% of ticket width
                 float maxLogoWidth = Width * 0.7f;   //meduim logo
               // float maxLogoWidth = Width - 20;
                float scale = maxLogoWidth / logo.Width;

                float logoWidth = logo.Width * scale;

                float logoHeight = logo.Height * scale;

                float x = (Width - logoWidth) / 2;

                //canvas.DrawBitmap(
                //    logo,
                //new SKRect(
                //    x,
                //    y,
                //    x + logoWidth,
                //    y + logoHeight));

                var paint = new SKPaint
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High
                };

                canvas.DrawBitmap(
                    logo,
                    new SKRect(
                    x,
                    y,
                    x + logoWidth,
                    y + logoHeight),
                    paint);

                // Margin below logo
                y += (int)logoHeight + LogoBottomMargin; ;
            }

            #endregion

            #region Title

            float titleWidth =
     titlePaint.MeasureText(t["TicketTitle"]);

            canvas.DrawText(
                t["TicketTitle"],
                (Width - titleWidth) / 2,
                y,
                titlePaint);

            y += 50;

            DrawLine(canvas, y);

            y += 30;

            #endregion
           
            DrawWrappedText(canvas, textPaint,
             $"{t["BookingNo"]}: {ticket.BookingNo}", ref y);


            DrawWrappedText(canvas, textPaint,
            $"{t["Client"]}: {ticket.client_name}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["Nationality"]}: {ticket.client_nationality}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["Phone"]}: {ticket.client_phone}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["TripName"]}: {ticket.trip_name}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["TripDate"]}: {ticket.trip_date:dd/MM/yyyy}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["Pickup"]}: {ticket.pickup_address}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["Drop"]}: {ticket.drop_address}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["Pax"]}: {ticket.total_pax}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["TripType"]}: {(ticket.is_two_way == true ? t["RoundTrip"] : t["OneWay"])}", ref y);

            DrawWrappedText(canvas, textPaint,
                $"{t["Amount"]}: {ticket.Amount:N2} {ticket.currency_code?.ToUpper()}", ref y);


            DrawWrappedText(canvas, textPaint,
               $"{t["Notes"]}: {ticket.notes}", ref y);


            //DrawWrappedText(canvas, textPaint,
            //    $"Booking No: {ticket.BookingNo}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Client: {ticket.client_name}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Nationality: {ticket.client_nationality}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Phone: {ticket.client_phone}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Trip Name: {ticket.trip_name}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Trip Date: {ticket.trip_date:dd/MM/yyyy}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Pickup: {ticket.pickup_address}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Drop: {ticket.drop_address}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Pax: {ticket.total_pax}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Trip Type: {(ticket.is_two_way == true ? "Round Trip" : "One Way")}", ref y);

            //DrawWrappedText(canvas, textPaint,
            //    $"Amount: {ticket.Amount:N2} {ticket.currency_code.ToUpper()}", ref y);

            //if (!string.IsNullOrWhiteSpace(ticket.notes))
            //{
            //    DrawWrappedText(canvas, textPaint,
            //        $"Notes: {ticket.notes}", ref y);
            //}

            y += 10;

            DrawLine(canvas, y);

            y += 30;

         

            using var image =
                SKImage.FromBitmap(bitmap);

            using var data =
                image.Encode(
                    SKEncodedImageFormat.Png,
                    100);

            return data.ToArray();
        }

        private int CalculateHeight(
            BookingTicketDto ticket,
            SKPaint paint,
            string logoPath)
        {
            var t = TicketTranslations.Get(ticket.lang_code);
            int height = 200;

            if (File.Exists(logoPath))
            {
                using var logo = SKBitmap.Decode(logoPath);

                float scale =
                    (float)Width / logo.Width;

                height +=
                    (int)(logo.Height * scale);
            }
            var fields = new[]
            {
                $"{t["BookingNo"]}: {ticket.BookingNo}",
                $"{t["Client"]}: {ticket.client_name}",
                $"{t["Nationality"]}: {ticket.client_nationality}",
                $"{t["Phone"]}: {ticket.client_phone}",
                $"{t["TripName"]}: {ticket.trip_name}",
                $"{t["TripDate"]}: {ticket.trip_date:dd/MM/yyyy}",
                $"{t["Pickup"]}: {ticket.pickup_address}",
                $"{t["Drop"]}: {ticket.drop_address}",
                $"{t["Pax"]}: {ticket.total_pax}",
                $"{t["TripType"]}: {(ticket.is_two_way == true ? t["RoundTrip"] : t["OneWay"])}",
                $"{t["Amount"]}: {ticket.Amount:N2} {ticket.currency_code}",
                $"{t["Notes"]}: {ticket.notes}"
            };
            //var fields = new[]
            //{
            //$"Booking No: {ticket.BookingNo}",
            //$"Client: {ticket.client_name}",
            //$"Nationality: {ticket.client_nationality}",
            //$"Phone: {ticket.client_phone}",
            //$"Trip Name: {ticket.trip_name}",
            //$"Trip Date: {ticket.trip_date:dd/MM/yyyy HH:mm}",
            //$"Pickup: {ticket.pickup_address}",
            //$"Drop: {ticket.drop_address}",
            //$"Pax: {ticket.total_pax}",
            //$"Trip Type: {(ticket.is_two_way == true ? "Round Trip" : "One Way")}",
            //$"Amount: {ticket.Amount:N2} {ticket.currency_code}",
            //$"Notes: {ticket.notes}"
        //};

            foreach (var field in fields)
            {
                if (string.IsNullOrWhiteSpace(field))
                    continue;

                int lines =
                    EstimateLines(field, paint);

                height +=
                    lines * 45;
            }

            return height + 100;
        }

        private int EstimateLines(
            string text,
            SKPaint paint)
        {
            float maxWidth =
                Width - (Margin * 2);

            float textWidth =
                paint.MeasureText(text);

            return Math.Max(
                1,
                (int)Math.Ceiling(
                    textWidth / maxWidth));
        }

        private void DrawWrappedText(
            SKCanvas canvas,
            SKPaint paint,
            string text,
            ref int y)
        {
            float maxWidth =
                Width - (Margin * 2);

            string[] words =
                text.Split(' ');

            string line = "";

            foreach (var word in words)
            {
                string testLine =
                    string.IsNullOrEmpty(line)
                        ? word
                        : $"{line} {word}";

                if (paint.MeasureText(testLine) > maxWidth)
                {
                    canvas.DrawText(
                        line,
                        Margin,
                        y,
                        paint);

                    y += 35;

                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }

            if (!string.IsNullOrEmpty(line))
            {
                canvas.DrawText(
                    line,
                    Margin,
                    y,
                    paint);

                y += 35;
            }

            y += RowSpacing;
        }

        private void DrawLine(
            SKCanvas canvas,
            int y)
        {
            canvas.DrawLine(
                Margin,
                y,
                Width - Margin,
                y,
                new SKPaint
                {
                    Color = SKColors.Black,
                    StrokeWidth = 2
                });
        }

       // private static byte[] GenerateQr(
        //    string value)
        //{
        //    var qrGenerator =
        //        new QRCodeGenerator();

        //    var qrData =
        //        qrGenerator.CreateQrCode(
        //            value,
        //            QRCodeGenerator.ECCLevel.Q);

        //    var qrCode =
        //        new PngByteQRCode(qrData);

        //    return qrCode.GetGraphic(10);
        //}
    
}
}
