namespace ActivityClub.API.DTOs.EventGuides
{
    public class EventGuideResponseDto
    {
        public int EventGuideId { get; set; }
        public int EventId { get; set; }
        public int GuideId { get; set; }
        public string GuideName { get; set; } = null!;
        public DateOnly AssignedDate { get; set; }
    }
}
