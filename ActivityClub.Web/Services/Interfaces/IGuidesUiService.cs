using ActivityClub.Web.ViewModels.Guides;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IGuidesUiService
    {
        Task<List<GuideListItemVm>> GetListAsync(CancellationToken ct = default);
        Task<GuideDetailsVm?> GetDetailsAsync(int id, CancellationToken ct = default);
    }
}
