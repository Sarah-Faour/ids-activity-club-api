using ActivityClub.Contracts.DTOs.Events;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IEventApiClient
    {
        Task<List<EventResponseDto>> GetAllAsync(CancellationToken ct = default);
        Task<EventResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
