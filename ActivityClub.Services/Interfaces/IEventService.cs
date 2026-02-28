using ActivityClub.Contracts.DTOs.Events;

namespace ActivityClub.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetAllAsync();
        Task<EventResponseDto?> GetByIdAsync(int id);
        Task<EventResponseDto> CreateAsync(CreateEventDto dto);
        Task<bool> UpdateAsync(int id, UpdateEventDto dto);
        Task<bool> SoftDeleteAsync(int id);

        //for admin only
        Task<List<EventResponseDto>> GetAllForAdminAsync();
        Task<bool> ReactivateAsync(int id);
    }
}
