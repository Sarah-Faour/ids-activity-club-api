using ActivityClub.Web.ViewModels.GuideProfile;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IGuideProfileUiService
    {
        Task<GuideProfileVm?> GetProfileAsync(CancellationToken ct = default);
        Task<EditGuideProfileVm?> GetEditVmAsync(CancellationToken ct = default);
        Task UpdateAsync(EditGuideProfileVm vm, CancellationToken ct = default);
    }
}