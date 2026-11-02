using ITravel_App.Models;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

namespace ITravel_App.Services.PrinterService
{
    public class BookingTicketDocument 
        //:  IDocument
    {
        private readonly BookingTicketDto _ticket;
        private readonly string _logoPath;

        //public BookingTicketDocument(
        //    BookingTicketDto ticket,
        //    string logoPath)
        //{
        //    _ticket = ticket;
        //    _logoPath = logoPath;
        //}

        //public DocumentMetadata GetMetadata()
        //    => DocumentMetadata.Default;

        //public void Compose(
        //    IDocumentContainer container)
        //{
        //    container.Page(page =>
        //    {
        //        page.Size(new PageSize(226.77f, 900));
        //        page.Margin(10);

        //        page.Content().Column(column =>
        //        {
        //            column.Item()
        //                .AlignCenter()
        //                .Image(_logoPath);

        //            column.Item()
        //                .PaddingTop(5)
        //                .AlignCenter()
        //                .Text("BOOKING TICKET")
        //                .Bold()
        //                .FontSize(16);

        //            column.Item()
        //                .PaddingVertical(5)
        //                .LineHorizontal(1);

        //            AddField(column, "Booking No", _ticket.BookingNo);
        //            AddField(column, "Client", _ticket.client_name);
        //            AddField(column, "Nationality", _ticket.client_nationality);
        //            AddField(column, "Phone", _ticket.client_phone);

        //            AddField(column, "Trip", _ticket.trip_name);

        //            column.Item().PaddingTop(5);

        //            column.Item()
        //                .Text("Pickup Address")
        //                .Bold();

        //            column.Item()
        //                .Text(_ticket.pickup_address);

        //            column.Item().PaddingTop(5);

        //            column.Item()
        //                .Text("Drop Address")
        //                .Bold();

        //            column.Item()
        //                .Text(_ticket.drop_address);

        //            AddField(
        //                column,
        //                "Trip Date",
        //                _ticket.trip_date.ToString(
        //                    "dd/MM/yyyy HH:mm"));

        //            AddField(
        //                column,
        //                "Passengers",
        //                _ticket.total_pax?.ToString());

        //            AddField(
        //                column,
        //                "Type",
        //                _ticket.is_two_way == true
        //                    ? "Round Trip"
        //                    : "One Way");

        //            AddField(
        //                column,
        //                "Amount",
        //                $"{_ticket.Amount:N2} {_ticket.currency_code}");

        //            column.Item()
        //                .PaddingVertical(5)
        //                .LineHorizontal(1);

        //            column.Item()
        //                .AlignCenter()
        //                .Text("Thank You")
        //                .Bold();
        //        });
        //    });
        //}

        //private void AddField(
        //    ColumnDescriptor column,
        //    string label,
        //    string? value)
        //{
        //    if (string.IsNullOrWhiteSpace(value))
        //        return;

        //    column.Item().Row(row =>
        //    {
        //        row.ConstantItem(80)
        //            .Text($"{label}:")
        //            .SemiBold();

        //        row.RelativeItem()
        //            .Text(value);
        //    });
        //}
    }
}
