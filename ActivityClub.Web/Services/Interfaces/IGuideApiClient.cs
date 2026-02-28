using ActivityClub.Contracts.DTOs.Guides;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IGuideApiClient
    {
        //public pages
        Task<List<GuideResponseDto>> GetAllAsync(CancellationToken ct = default);
        //both public and admin 
        Task<GuideResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);

        //Admin things
        Task<GuideResponseDto> CreateAsync(CreateGuideDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateGuideDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
        Task<List<GuideResponseDto>> GetAllForAdminAsync(CancellationToken ct = default);
    }
}
