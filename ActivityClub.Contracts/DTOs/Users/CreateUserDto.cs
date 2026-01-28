using System.ComponentModel.DataAnnotations;

namespace ActivityClub.Contracts.DTOs.Users
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public int GenderLookupId { get; set; }
    }
}
