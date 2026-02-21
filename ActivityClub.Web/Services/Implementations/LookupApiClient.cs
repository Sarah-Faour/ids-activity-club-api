using System.Net.Http.Json;
using ActivityClub.Contracts.DTOs.Lookups;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class LookupApiClient : ILookupApiClient
    {
        private readonly HttpClient _http;

        public LookupApiClient(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ActivityClubApi");
        }

        public async Task<IReadOnlyList<LookupResponseDto>> GetByCodeAsync(string code, CancellationToken ct = default)
        {
            var items = await _http.GetFromJsonAsync<List<LookupResponseDto>>(
                $"api/lookups/code/{code}", ct);

            return items ?? new List<LookupResponseDto>();
        }
    }
}