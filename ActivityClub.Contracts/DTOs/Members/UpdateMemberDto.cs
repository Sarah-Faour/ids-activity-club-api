namespace ActivityClub.Contracts.DTOs.Members
{
    public class UpdateMemberDto
    {
        public string FullName { get; set; } = null!;

        public string? MobileNumber { get; set; }
        public string? EmergencyNumber { get; set; }

        public DateOnly JoiningDate { get; set; }
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }
        public int? NationalityId { get; set; }
    }
}
