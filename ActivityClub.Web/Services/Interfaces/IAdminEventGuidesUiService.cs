using ActivityClub.Web.ViewModels.Admin;

namespace ActivityClub.Web.Services.Interfaces;

public interface IAdminEventGuidesUiService
{
    Task LoadAssignmentsAsync(int eventId, AdminEventEditVm vm, CancellationToken ct = default);
    Task<bool> AssignAsync(AssignGuideVm vm, CancellationToken ct = default);
    Task<bool> UnassignAsync(int eventId, int guideId, CancellationToken ct = default);
}