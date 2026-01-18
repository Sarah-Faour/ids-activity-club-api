namespace ActivityClub.API.DTOs.Guides
{
    public class GuideResponseDto
    {
        public int GuideId { get; set; }
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public DateOnly JoiningDate { get; set; }
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }
        public string? ProfessionName { get; set; }

        public bool IsActive { get; set; }
    }
}
