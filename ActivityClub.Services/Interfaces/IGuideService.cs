using ActivityClub.Contracts.DTOs.Guides;

namespace ActivityClub.Services.Interfaces
{
    public interface IGuideService
    {
        //public pages
        Task<List<GuideResponseDto>> GetAllAsync();
        //for both public and admin
        Task<GuideResponseDto?> GetByIdAsync(int id);

        //admin only pages
        Task<GuideResponseDto> CreateAsync(CreateGuideDto dto);
        Task<bool> UpdateAsync(int id, UpdateGuideDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ReactivateAsync(int id);
        Task<List<GuideResponseDto>> GetAllForAdminAsync();

        // ✅ Self-service
        Task<GuideResponseDto?> GetMyProfileAsync(int userId);
        Task<bool> UpdateMyProfileAsync(int userId, UpdateGuideDto dto);
    }
}
