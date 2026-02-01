using ActivityClub.Contracts.DTOs.EventGuides;

namespace ActivityClub.Services.Interfaces
{
    public interface IEventGuideService
    {
        Task<List<EventGuideResponseDto>?> GetGuidesForEventAsync(int eventId);
        Task<EventGuideResponseDto> AssignGuideToEventAsync(int eventId, AssignGuideDto dto);
        Task<bool> UnassignGuideFromEventAsync(int eventId, int guideId);
    }
}

