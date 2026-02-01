using ActivityClub.Contracts.DTOs.Roles;

namespace ActivityClub.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleResponseDto>> GetAllAsync();
        Task<RoleResponseDto?> GetByIdAsync(int id);
    }
}
