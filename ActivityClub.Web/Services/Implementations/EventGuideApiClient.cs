using System.Net.Http.Json;
using ActivityClub.Contracts.DTOs.EventGuides;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations;

public sealed class EventGuideApiClient : IEventGuideApiClient
{
    private readonly HttpClient _http;

    public EventGuideApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ActivityClubApi");
    }

    public async Task<List<EventGuideResponseDto>> GetForEventAsync(int eventId, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<EventGuideResponseDto>>(
            $"api/events/{eventId}/guides", ct);

        return result ?? new List<EventGuideResponseDto>();
    }

    public async Task<EventGuideResponseDto> AssignAsync(int eventId, AssignGuideDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"api/events/{eventId}/guides", dto, ct);
        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<EventGuideResponseDto>(cancellationToken: ct))!;
    }

    public async Task<bool> UnassignAsync(int eventId, int guideId, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/events/{eventId}/guides/{guideId}", ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<List<EventGuideAdminResponseDto>> GetForEventForAdminAsync(int eventId, CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<EventGuideAdminResponseDto>>(
            $"api/events/{eventId}/guides/admin", ct);

        return result ?? new List<EventGuideAdminResponseDto>();
    }
}