using ActivityClub.Web.ViewModels.Admin;

namespace ActivityClub.Web.Services.Interfaces;

public interface IAdminGuidesUiService
{
    Task<List<AdminGuideListVm>> GetAllAsync(CancellationToken ct = default);
    Task<AdminGuideEditVm?> GetByIdAsync(int id, CancellationToken ct = default);

    Task CreateAsync(AdminGuideEditVm vm, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, AdminGuideEditVm vm, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
}