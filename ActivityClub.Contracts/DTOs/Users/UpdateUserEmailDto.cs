using System.ComponentModel.DataAnnotations;

namespace ActivityClub.Contracts.DTOs.Users
{
    public class UpdateUserEmailDto
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;
    }
}

