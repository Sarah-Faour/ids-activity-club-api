using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Events;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class EventsUiService : IEventsUiService
    {
        private readonly IEventApiClient _eventApiClient;
        private readonly IEventGuideApiClient _eventGuideApiClient; // ✅ NEW

        public EventsUiService(IEventApiClient eventApiClient, IEventGuideApiClient eventGuideApiClient)
        {
            _eventApiClient = eventApiClient;
            _eventGuideApiClient = eventGuideApiClient; // ✅ NEW
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

            // ✅ NEW: active guides assigned to this event
            var guides = await _eventGuideApiClient.GetForEventAsync(eventId, ct);

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
                StatusName = e.StatusName,

                Guides = guides // ✅ NEW
            };
        }

        public async Task JoinEventAsync(int eventId, CancellationToken ct = default)
        {
            await _eventApiClient.JoinAsync(eventId, ct);
        }


    }
}
