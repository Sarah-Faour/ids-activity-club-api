namespace ActivityClub.Contracts.DTOs.Members
{
    public class MemberResponseDto
    {
        public int MemberId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;

        public string? MobileNumber { get; set; }
        public string? EmergencyNumber { get; set; }

        public DateOnly JoiningDate { get; set; }
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }
        public string? ProfessionName { get; set; }

        public int? NationalityId { get; set; }
        public string? NationalityName { get; set; }

        public bool IsActive { get; set; } // from member
    }
}
