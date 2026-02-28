using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.ViewModels.Admin;

public sealed class AdminGuideEditVm
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public DateOnly JoiningDate { get; set; }

    public string? Photo { get; set; }

    public int? ProfessionId { get; set; }

    // UI-only dropdown
    public IReadOnlyList<SelectListItem> ProfessionOptions { get; set; }
        = Array.Empty<SelectListItem>();
}