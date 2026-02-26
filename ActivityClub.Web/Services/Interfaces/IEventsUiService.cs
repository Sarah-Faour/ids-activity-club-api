using ActivityClub.Web.ViewModels.Events;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IEventsUiService
    {
        Task<IReadOnlyList<EventListItemVm>> GetAllAsync(CancellationToken ct = default);
        Task<EventDetailsVm?> GetByIdAsync(int eventId, CancellationToken ct = default);

        // NEW
        Task JoinEventAsync(int eventId, CancellationToken ct = default);
    }
}
