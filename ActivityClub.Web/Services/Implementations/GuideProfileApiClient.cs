using System.Net.Http.Json;
using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class GuideProfileApiClient : IGuideProfileApiClient
    {
        private readonly HttpClient _http;

        public GuideProfileApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ActivityClubApi");
        }

        public async Task<GuideResponseDto?> GetMeAsync(CancellationToken ct = default)
        {
            var res = await _http.GetAsync("api/guides/me", ct);

            if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<GuideResponseDto>(cancellationToken: ct);
        }

        public async Task UpdateMeAsync(UpdateGuideDto dto, CancellationToken ct = default)
        {
            var res = await _http.PutAsJsonAsync("api/guides/me", dto, ct);
            res.EnsureSuccessStatusCode();
        }
    }
}