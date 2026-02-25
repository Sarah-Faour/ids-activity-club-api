using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.ViewModels.GuideProfile
{
    public class EditGuideProfileVm
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        // Not editable by user, but required by API DTO
        [Required]
        public string JoiningDate { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }

        public IReadOnlyList<SelectListItem> ProfessionOptions { get; set; } = new List<SelectListItem>();
    }
}