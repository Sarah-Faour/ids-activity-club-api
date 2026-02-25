using ActivityClub.Contracts.DTOs.Guides;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IGuideProfileApiClient
    {
        Task<GuideResponseDto?> GetMeAsync(CancellationToken ct = default);
        Task UpdateMeAsync(UpdateGuideDto dto, CancellationToken ct = default);
    }
}