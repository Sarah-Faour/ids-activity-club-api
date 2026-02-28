using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.ViewModels.Admin;

public sealed class AdminEventEditVm
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public string Destination { get; set; } = null!;

    [Required]
    public DateOnly DateFrom { get; set; }

    [Required]
    public DateOnly DateTo { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }

    [Required]
    public int StatusId { get; set; }

    // UI-only dropdowns (not sent to API)
    public IReadOnlyList<SelectListItem> CategoryOptions { get; set; }
        = Array.Empty<SelectListItem>();

    public IReadOnlyList<SelectListItem> StatusOptions { get; set; }
        = Array.Empty<SelectListItem>();
}