namespace ITravel_App.Models
{
    public static class TicketTranslations
    {
        public static Dictionary<string, string> Get(string? lang)
        {
            lang = (lang ?? "en").ToLower();

            return lang switch
            {
                "de" => German,
                "ru" => Russian,
                _ => English
            };
        }

        private static readonly Dictionary<string, string> English = new()
        {
            ["TicketTitle"] = "BOOKING TICKET",
            ["BookingNo"] = "Booking No",
            ["Client"] = "Client",
            ["Nationality"] = "Nationality",
            ["Phone"] = "Phone",
            ["TripName"] = "Trip Name",
            ["TripDate"] = "Trip Date",
            ["Pickup"] = "Pickup",
            ["Drop"] = "Drop",
            ["Pax"] = "Pax",
            ["TripType"] = "Trip Type",
            ["RoundTrip"] = "Round Trip",
            ["OneWay"] = "One Way",
            ["Amount"] = "Amount",
            ["Notes"] = "Notes"
        };

        private static readonly Dictionary<string, string> German = new()
        {
            ["TicketTitle"] = "BUCHUNGSTICKET",
            ["BookingNo"] = "Buchungsnummer",
            ["Client"] = "Kunde",
            ["Nationality"] = "Nationalität",
            ["Phone"] = "Telefon",
            ["TripName"] = "Ausflug",
            ["TripDate"] = "Datum",
            ["Pickup"] = "Abholung",
            ["Drop"] = "Ziel",
            ["Pax"] = "Personen",
            ["TripType"] = "Fahrttyp",
            ["RoundTrip"] = "Hin- und Rückfahrt",
            ["OneWay"] = "Einfache Fahrt",
            ["Amount"] = "Betrag",
            ["Notes"] = "Notizen"
        };

        private static readonly Dictionary<string, string> Russian = new()
        {
            ["TicketTitle"] = "БИЛЕТ БРОНИРОВАНИЯ",
            ["BookingNo"] = "Номер бронирования",
            ["Client"] = "Клиент",
            ["Nationality"] = "Гражданство",
            ["Phone"] = "Телефон",
            ["TripName"] = "Экскурсия",
            ["TripDate"] = "Дата",
            ["Pickup"] = "Место посадки",
            ["Drop"] = "Место высадки",
            ["Pax"] = "Пассажиры",
            ["TripType"] = "Тип поездки",
            ["RoundTrip"] = "Туда и обратно",
            ["OneWay"] = "В одну сторону",
            ["Amount"] = "Сумма",
            ["Notes"] = "Примечания"
        };
    }
}
