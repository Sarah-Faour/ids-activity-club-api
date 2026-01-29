using ActivityClub.Contracts.DTOs.Guides;

namespace ActivityClub.Services.Interfaces
{
    public interface IGuideService
    {
        Task<List<GuideResponseDto>> GetAllAsync();
        Task<GuideResponseDto?> GetByIdAsync(int id);
        Task<GuideResponseDto> CreateAsync(CreateGuideDto dto);
        Task<bool> UpdateAsync(int id, UpdateGuideDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}
