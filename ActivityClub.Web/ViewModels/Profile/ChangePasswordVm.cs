using System.ComponentModel.DataAnnotations;

namespace ActivityClub.Web.ViewModels.Profile
{
    public sealed class ChangePasswordVm
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;
    }
}