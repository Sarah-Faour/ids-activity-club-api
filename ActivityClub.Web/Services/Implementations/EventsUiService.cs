using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Events;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class EventsUiService : IEventsUiService
    {
        private readonly IEventApiClient _eventApiClient;

        public EventsUiService(IEventApiClient eventApiClient)
        {
            _eventApiClient = eventApiClient;
        }

        public async Task<IReadOnlyList<EventListItemVm>> GetAllAsync(CancellationToken ct = default)
        {
            var dtos = await _eventApiClient.GetAllAsync(ct);

            return dtos.Select(e => new EventListItemVm
            {
                EventId = e.EventId,
                Name = e.Name,
                Destination = e.Destination,
                DateFromText = e.DateFrom.ToString("yyyy-MM-dd"),
                DateToText = e.DateTo.ToString("yyyy-MM-dd"),
                Cost = e.Cost,
                CategoryName = e.CategoryName,
                StatusName = e.StatusName
            }).ToList();
        }

        public async Task<EventDetailsVm?> GetByIdAsync(int eventId, CancellationToken ct = default)
        {
            var e = await _eventApiClient.GetByIdAsync(eventId, ct);
            if (e == null) return null;

            return new EventDetailsVm
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                Destination = e.Destination,
                // ✅ MUST populate (needed for “upcoming” logic)
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                DateFromText = e.DateFrom.ToString("yyyy-MM-dd"),
                DateToText = e.DateTo.ToString("yyyy-MM-dd"),
                Cost = e.Cost,
                CategoryName = e.CategoryName,
                StatusName = e.StatusName
            };
        }

        public async Task JoinEventAsync(int eventId, CancellationToken ct = default)
        {
            await _eventApiClient.JoinAsync(eventId, ct);
        }


    }
}
