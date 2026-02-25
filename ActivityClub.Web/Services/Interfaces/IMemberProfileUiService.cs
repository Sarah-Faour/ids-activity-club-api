using ActivityClub.Web.ViewModels.MemberProfile;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IMemberProfileUiService
    {
        Task<MemberProfileVm> GetProfileAsync(CancellationToken ct = default);
        Task<EditMemberProfileVm> GetEditVmAsync(CancellationToken ct = default);
        Task UpdateAsync(EditMemberProfileVm vm, CancellationToken ct = default);
    }
}