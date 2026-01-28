using System.ComponentModel.DataAnnotations;

namespace ActivityClub.Contracts.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public int GenderLookupId { get; set; }
    }
}
