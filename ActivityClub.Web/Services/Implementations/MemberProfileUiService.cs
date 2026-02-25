using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.MemberProfile;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class MemberProfileUiService : IMemberProfileUiService
    {
        private readonly IMemberProfileApiClient _api;

        public MemberProfileUiService(IMemberProfileApiClient api)
        {
            _api = api;
        }

        public async Task<MemberProfileVm> GetProfileAsync(CancellationToken ct = default)
        {
            var me = await _api.GetMeAsync(ct);

            return new MemberProfileVm
            {
                FullName = me.FullName,
                MobileNumber = me.MobileNumber,
                EmergencyNumber = me.EmergencyNumber,
                JoiningDateText = me.JoiningDate.ToString("yyyy-MM-dd"),
                ProfessionName = me.ProfessionName,
                NationalityName = me.NationalityName,
                Photo = me.Photo,
                IsActive = me.IsActive
            };
        }

        public async Task<EditMemberProfileVm> GetEditVmAsync(CancellationToken ct = default)
        {
            var me = await _api.GetMeAsync(ct);

            return new EditMemberProfileVm
            {
                FullName = me.FullName,
                MobileNumber = me.MobileNumber,
                EmergencyNumber = me.EmergencyNumber,
                JoiningDate = me.JoiningDate,
                Photo = me.Photo,
                ProfessionId = me.ProfessionId,
                NationalityId = me.NationalityId
            };
        }

        public async Task UpdateAsync(EditMemberProfileVm vm, CancellationToken ct = default)
        {
            var dto = new UpdateMemberDto
            {
                FullName = vm.FullName,
                MobileNumber = vm.MobileNumber,
                EmergencyNumber = vm.EmergencyNumber,
                JoiningDate = vm.JoiningDate,
                Photo = vm.Photo,
                ProfessionId = vm.ProfessionId,
                NationalityId = vm.NationalityId
            };

            await _api.UpdateMeAsync(dto, ct);
        }
    }
}