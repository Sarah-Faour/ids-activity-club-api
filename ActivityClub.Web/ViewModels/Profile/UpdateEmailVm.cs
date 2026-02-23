using System.ComponentModel.DataAnnotations;

namespace ActivityClub.Web.ViewModels.Profile
{
    public sealed class UpdateEmailVm
    {
        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = null!;
    }
}
