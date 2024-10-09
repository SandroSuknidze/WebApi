namespace WebApi.Models
{
    public class Card
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Photo { get; set; }
        public required int AvailableUnits { get; set; }
        public required bool Wifi { get; set; }
        public required bool Laundry { get; set; }
        public required string Pdf { get; set; }
    }
}
