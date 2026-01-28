namespace ActivityClub.Contracts.DTOs.Guides
{
    public class CreateGuideDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public DateOnly JoiningDate { get; set; }
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }
    }
}
