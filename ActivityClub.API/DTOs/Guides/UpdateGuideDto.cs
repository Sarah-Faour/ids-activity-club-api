namespace ActivityClub.API.DTOs.Guides
{
    public class UpdateGuideDto
    {
        public string FullName { get; set; } = null!;

        public DateOnly JoiningDate { get; set; }
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }
    }
}
