using ActivityClub.Contracts.DTOs.Roles;

namespace ActivityClub.Contracts.DTOs.Users
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }

        public int GenderLookupId { get; set; }
        public string GenderName { get; set; } = null!;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<RoleResponseDto> Roles { get; set; } = new();
    }
}
