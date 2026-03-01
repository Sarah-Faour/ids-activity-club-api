using ActivityClub.Contracts.DTOs.EventGuides;

namespace ActivityClub.Services.Interfaces
{
    public interface IEventGuideService
    {
        //for public pages
        Task<List<EventGuideResponseDto>?> GetGuidesForEventAsync(int eventId);

        //Admin-only 
        Task<EventGuideResponseDto> AssignGuideToEventAsync(int eventId, AssignGuideDto dto);
        Task<bool> UnassignGuideFromEventAsync(int eventId, int guideId);
        Task<List<EventGuideAdminResponseDto>?> GetGuidesForEventForAdminAsync(int eventId);
    }
}

