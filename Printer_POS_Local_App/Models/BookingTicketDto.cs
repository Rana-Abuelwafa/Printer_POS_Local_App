namespace ITravel_App.Models
{
    public class BookingTicketDto
    {
        public string? BookingNo { get; set; }
        public string? client_name { get; set; }
        public string? pickup_address { get; set; }
        public string? drop_address { get; set; }
        public string? trip_name { get; set; }
        public DateTime trip_date { get; set; }
        public decimal Amount { get; set; }
        public string? currency_code { get; set; }
        public bool? is_two_way { get; set; }
        public string? client_nationality { get; set; }
        public string? client_phone { get; set; }
        public int? total_pax { get; set; }
        public string? notes { get; set; }
        public string? lang_code { get; set; }
    }
}
