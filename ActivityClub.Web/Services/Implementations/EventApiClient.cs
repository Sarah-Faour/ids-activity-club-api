using System.Net.Http.Json;
using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web.Services.Implementations;

public class EventApiClient : IEventApiClient
{
    private readonly HttpClient _http;

    public EventApiClient(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("ActivityClubApi");
    }

    public async Task<List<EventResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<EventResponseDto>>("api/events", ct);
        return result ?? new List<EventResponseDto>();
    }

    public async Task<EventResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<EventResponseDto>($"api/events/{id}", ct);
    }

    //member joining
    public async Task JoinAsync(int eventId, CancellationToken ct = default)
    {
        var res = await _http.PostAsync($"api/events/{eventId}/join", content: null, ct);
        res.EnsureSuccessStatusCode();
    }

    //Admin things

    public async Task<EventResponseDto> CreateAsync(CreateEventDto dto, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/events", dto, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<EventResponseDto>(cancellationToken: ct))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateEventDto dto, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/events/{id}", dto, ct);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/events/{id}", ct);
        return res.IsSuccessStatusCode;
    }
    public async Task<List<EventResponseDto>> GetAllForAdminAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<EventResponseDto>>("api/events/admin", ct);
        return result ?? new List<EventResponseDto>();
    }

    public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.PostAsync($"api/events/{id}/reactivate", content: null, ct);
        return res.IsSuccessStatusCode;
    }
}