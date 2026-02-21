using ActivityClub.Contracts.DTOs.Auth;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IAuthApiClient
    {
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
        Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);
    }
}