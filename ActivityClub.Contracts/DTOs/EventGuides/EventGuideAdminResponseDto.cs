namespace ActivityClub.Contracts.DTOs.EventGuides
{
    public class EventGuideAdminResponseDto
    {
        public int EventGuideId { get; set; }
        public int EventId { get; set; }
        public bool EventIsActive { get; set; }

        public int GuideId { get; set; }
        public string GuideName { get; set; } = null!;
        public bool GuideIsActive { get; set; }
        public bool GuideUserIsActive { get; set; }

        public DateOnly AssignedDate { get; set; }
    }
}