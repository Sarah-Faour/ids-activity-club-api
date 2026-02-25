using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.GuideProfile;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class GuideProfileUiService : IGuideProfileUiService
    {
        private readonly IGuideProfileApiClient _api;

        public GuideProfileUiService(IGuideProfileApiClient api)
        {
            _api = api;
        }

        public async Task<GuideProfileVm?> GetProfileAsync(CancellationToken ct = default)
        {
            var dto = await _api.GetMeAsync(ct);
            if (dto is null) return null;

            return new GuideProfileVm
            {
                GuideId = dto.GuideId,
                UserId = dto.UserId,
                FullName = dto.FullName,
                JoiningDateText = dto.JoiningDate.ToString("yyyy-MM-dd"),
                Photo = dto.Photo,
                ProfessionId = dto.ProfessionId,
                ProfessionName = dto.ProfessionName,
                IsActive = dto.IsActive
            };
        }

        public async Task<EditGuideProfileVm?> GetEditVmAsync(CancellationToken ct = default)
        {
            var dto = await _api.GetMeAsync(ct);
            if (dto is null) return null;

            return new EditGuideProfileVm
            {
                FullName = dto.FullName,
                JoiningDate = dto.JoiningDate.ToString("yyyy-MM-dd"),
                Photo = dto.Photo,
                ProfessionId = dto.ProfessionId
            };
        }

        public async Task UpdateAsync(EditGuideProfileVm vm, CancellationToken ct = default)
        {
            // DateOnly parse from yyyy-MM-dd
            if (!DateOnly.TryParse(vm.JoiningDate, out var joiningDate))
                throw new ArgumentException("Invalid joining date format.");

            var dto = new UpdateGuideDto
            {
                FullName = vm.FullName,
                JoiningDate = joiningDate,
                Photo = vm.Photo,
                ProfessionId = vm.ProfessionId
            };

            await _api.UpdateMeAsync(dto, ct);
        }
    }
}