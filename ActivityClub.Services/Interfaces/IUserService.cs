using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;

namespace ActivityClub.Services.Interfaces
{
    public interface IUserService
    {
        // Users CRUD
        Task<List<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<UserResponseDto> CreateAsync(CreateUserDto dto);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ReactivateAsync(int id);

        // Role assignment endpoints
        Task<List<RoleResponseDto>?> GetRolesAsync(int userId);
        Task<bool> AssignRoleAsync(int userId, int roleId);
        Task<bool> UnassignRoleAsync(int userId, int roleId);

        // (not required — better for Murex), changing Email/Pass safely
        Task<bool> UpdateEmailAsync(int id, UpdateUserEmailDto dto);
        Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto);
    }
}
