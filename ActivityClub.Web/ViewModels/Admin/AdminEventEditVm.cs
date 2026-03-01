using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ActivityClub.Contracts.DTOs.EventGuides;

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

    //adding Assigned guides table and Dropdown to assign a guide for admin managing 

    // ✅ NEW: assigned guides table (admin sees active + inactive)
    public List<EventGuideAdminResponseDto> AssignedGuides { get; set; } = new();

    // ✅ NEW: dropdown for assigning (active guides only)
    public IReadOnlyList<SelectListItem> GuideOptions { get; set; } = Array.Empty<SelectListItem>();

    // ✅ NEW: selected guide for assign dropdown (binds from <select>)
    public int SelectedGuideId { get; set; }
}