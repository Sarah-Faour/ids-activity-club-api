using ActivityClub.Contracts.DTOs.Guides;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IGuideApiClient
    {
        Task<List<GuideResponseDto>> GetAllAsync(CancellationToken ct = default);
        Task<GuideResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
