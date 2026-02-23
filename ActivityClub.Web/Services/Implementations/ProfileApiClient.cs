using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class ProfileApiClient : IProfileApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UserResponseDto> GetMeAsync(CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.GetAsync("api/users/me", ct);
            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<UserResponseDto>(cancellationToken: ct))!;
        }

        public async Task<List<RoleResponseDto>> GetMyRolesAsync(CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.GetAsync("api/users/me/roles", ct);
            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<List<RoleResponseDto>>(cancellationToken: ct)) ?? new();
        }

        public async Task UpdateMyEmailAsync(UpdateUserEmailDto dto, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.PutAsJsonAsync("api/users/me/email", dto, ct);
            res.EnsureSuccessStatusCode();
        }

        public async Task ChangeMyPasswordAsync(ChangePasswordDto dto, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.PutAsJsonAsync("api/users/me/password", dto, ct);
            res.EnsureSuccessStatusCode();
        }
    }
}