using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IProfileApiClient
    {
        Task<UserResponseDto> GetMeAsync(CancellationToken ct = default);
        Task<List<RoleResponseDto>> GetMyRolesAsync(CancellationToken ct = default);
        Task UpdateMyEmailAsync(UpdateUserEmailDto dto, CancellationToken ct = default);
        Task ChangeMyPasswordAsync(ChangePasswordDto dto, CancellationToken ct = default);
    }
}