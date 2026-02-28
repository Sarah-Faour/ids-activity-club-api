namespace ActivityClub.Web.ViewModels.Admin;

public sealed class AdminGuideListVm
{
    public int GuideId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public DateOnly JoiningDate { get; set; }
    public string? ProfessionName { get; set; }
    public string? Photo { get; set; }
    public bool IsActive { get; set; }
}