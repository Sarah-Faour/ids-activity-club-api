using ActivityClub.Web.ViewModels.Profile;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IProfileUiService
    {
        Task<ProfileVm> GetProfileAsync(CancellationToken ct = default);
        Task UpdateEmailAsync(UpdateEmailVm vm, CancellationToken ct = default);
        Task ChangePasswordAsync(ChangePasswordVm vm, CancellationToken ct = default);
    }
}