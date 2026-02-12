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
}