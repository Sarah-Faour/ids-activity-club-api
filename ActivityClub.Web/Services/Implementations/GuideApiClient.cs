using ActivityClub.Contracts.DTOs.Guides;
using System.Net;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class GuideApiClient : IGuideApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GuideApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<GuideResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");

            var res = await client.GetAsync("api/guides", ct);
            res.EnsureSuccessStatusCode();

            var data = await res.Content.ReadFromJsonAsync<List<GuideResponseDto>>(cancellationToken: ct);
            return data ?? new List<GuideResponseDto>();
        }

        public async Task<GuideResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");

            var res = await client.GetAsync($"api/guides/{id}", ct);

            if (res.StatusCode == HttpStatusCode.NotFound)
                return null;

            res.EnsureSuccessStatusCode();

            return await res.Content.ReadFromJsonAsync<GuideResponseDto>(cancellationToken: ct);
        }
    }
}