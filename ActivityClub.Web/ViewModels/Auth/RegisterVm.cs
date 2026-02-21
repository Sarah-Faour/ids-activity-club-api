using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.ViewModels.Auth
{
    public sealed class RegisterVm
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select a gender.")]
        public int? GenderLookupId { get; set; }

        // dropdown items
        public IReadOnlyList<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();
    }
}