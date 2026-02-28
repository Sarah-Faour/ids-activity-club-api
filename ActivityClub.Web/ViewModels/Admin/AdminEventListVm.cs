namespace ActivityClub.Web.ViewModels.Admin;

public sealed class AdminEventListVm
{
    public int EventId { get; set; }
    public string Name { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal Cost { get; set; }
    public string? CategoryName { get; set; }
    public string? StatusName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? IsActive { get; set; }
}