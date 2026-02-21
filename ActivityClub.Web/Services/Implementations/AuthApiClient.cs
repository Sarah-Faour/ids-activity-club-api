using System.Net;
using System.Net.Http.Json;
using ActivityClub.Contracts.DTOs.Auth;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _http;

        public AuthApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ActivityClubApi");
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/login", dto, ct);

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AuthResponseDto>(cancellationToken: ct);
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/auth/register", dto, ct);

            // register can fail with 400 or 500 depending on backend exception handling
            // we bubble errors (shows as exception) unless you want custom UI messages later
            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<AuthResponseDto>(cancellationToken: ct);
        }
    }
}