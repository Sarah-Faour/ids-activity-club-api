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

        //Admin things
        public async Task<GuideResponseDto> CreateAsync(CreateGuideDto dto, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");

            var res = await client.PostAsJsonAsync("api/guides", dto, ct);
            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<GuideResponseDto>(cancellationToken: ct))!;
        }

        public async Task<bool> UpdateAsync(int id, UpdateGuideDto dto, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.PutAsJsonAsync($"api/guides/{id}", dto, ct);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.DeleteAsync($"api/guides/{id}", ct);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.PostAsync($"api/guides/{id}/reactivate", content: null, ct);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<GuideResponseDto>> GetAllForAdminAsync(CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");

            var res = await client.GetAsync("api/guides/admin", ct);
            res.EnsureSuccessStatusCode();

            var data = await res.Content.ReadFromJsonAsync<List<GuideResponseDto>>(cancellationToken: ct);
            return data ?? new List<GuideResponseDto>();
        }
    }
}