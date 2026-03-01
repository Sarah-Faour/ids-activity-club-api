using ActivityClub.Contracts.DTOs.EventGuides;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.Services.Implementations;

public sealed class AdminEventGuidesUiService : IAdminEventGuidesUiService
{
    private readonly IEventGuideApiClient _eventGuidesApi;
    private readonly IGuideApiClient _guidesApi;

    public AdminEventGuidesUiService(IEventGuideApiClient eventGuidesApi, IGuideApiClient guidesApi)
    {
        _eventGuidesApi = eventGuidesApi;
        _guidesApi = guidesApi;
    }

    public async Task LoadAssignmentsAsync(int eventId, AdminEventEditVm vm, CancellationToken ct = default)
    {
        // 1) Assigned guides (admin sees all assignments, even inactive)
        vm.AssignedGuides = await _eventGuidesApi.GetForEventForAdminAsync(eventId, ct);

        // 2) Dropdown options (only ACTIVE guides)
        var activeGuides = await _guidesApi.GetAllAsync(ct);

        vm.GuideOptions = activeGuides
            .Select(g => new SelectListItem($"{g.FullName} (#{g.GuideId})", g.GuideId.ToString()))
            .ToList();
    }

    public async Task<bool> AssignAsync(AssignGuideVm vm, CancellationToken ct = default)
    {
        // API expects AssignGuideDto
        await _eventGuidesApi.AssignAsync(vm.EventId, new AssignGuideDto { GuideId = vm.GuideId }, ct);
        return true; // if API fails, it will throw (or you can return false based on status)
    }

    public Task<bool> UnassignAsync(int eventId, int guideId, CancellationToken ct = default)
        => _eventGuidesApi.UnassignAsync(eventId, guideId, ct);
}