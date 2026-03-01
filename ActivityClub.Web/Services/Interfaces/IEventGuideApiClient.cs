using ActivityClub.Contracts.DTOs.EventGuides;

namespace ActivityClub.Web.Services.Interfaces;

public interface IEventGuideApiClient
{
    //for public page
    Task<List<EventGuideResponseDto>> GetForEventAsync(int eventId, CancellationToken ct = default);

    //Admin-Only
    Task<EventGuideResponseDto> AssignAsync(int eventId, AssignGuideDto dto, CancellationToken ct = default);
    Task<bool> UnassignAsync(int eventId, int guideId, CancellationToken ct = default);
    Task<List<EventGuideAdminResponseDto>> GetForEventForAdminAsync(int eventId, CancellationToken ct = default);
}