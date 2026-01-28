namespace ActivityClub.Contracts.DTOs.Events
{
    public class CreateEventDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string Destination { get; set; } = null!;
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public decimal Cost { get; set; }
        public int StatusId { get; set; }
    }
}
