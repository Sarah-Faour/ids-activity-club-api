using ActivityClub.Web.ViewModels.Admin;

namespace ActivityClub.Web.Services.Interfaces;

public interface IAdminEventsUiService
{
    Task<List<AdminEventListVm>> GetAllAsync(CancellationToken ct = default);
    Task<AdminEventEditVm?> GetByIdAsync(int id, CancellationToken ct = default);

    Task CreateAsync(AdminEventEditVm vm, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, AdminEventEditVm vm, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
}