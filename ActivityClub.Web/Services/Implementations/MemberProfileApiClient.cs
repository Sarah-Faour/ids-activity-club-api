using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class MemberProfileApiClient : IMemberProfileApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MemberProfileApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MemberResponseDto> GetMeAsync(CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.GetAsync("api/members/me", ct);
            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<MemberResponseDto>(cancellationToken: ct))!;
        }

        public async Task UpdateMeAsync(UpdateMemberDto dto, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("ActivityClubApi");
            var res = await client.PutAsJsonAsync("api/members/me", dto, ct);
            res.EnsureSuccessStatusCode();
        }
    }
}