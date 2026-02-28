using ActivityClub.Contracts.DTOs.Events;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IEventApiClient
    {
        Task<List<EventResponseDto>> GetAllAsync(CancellationToken ct = default);
        Task<EventResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
        
        // NEW: member self-join
        Task JoinAsync(int eventId, CancellationToken ct = default);

        //Admin things
        Task<EventResponseDto> CreateAsync(CreateEventDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateEventDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<List<EventResponseDto>> GetAllForAdminAsync(CancellationToken ct = default);
        Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
    }
}
