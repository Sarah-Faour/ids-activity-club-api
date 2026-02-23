using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Profile;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class ProfileUiService : IProfileUiService
    {
        private readonly IProfileApiClient _api;

        public ProfileUiService(IProfileApiClient api)
        {
            _api = api;
        }

        public async Task<ProfileVm> GetProfileAsync(CancellationToken ct = default)
        {
            // 1) Fetch current user
            var me = await _api.GetMeAsync(ct);

            // 2) Fetch roles (separate endpoint in your API design)
            var roles = await _api.GetMyRolesAsync(ct);

            // 3) Map API DTOs -> Profile ViewModel (UI-friendly fields)
            return new ProfileVm
            {
                UserId = me.UserId,
                Name = me.Name,
                Email = me.Email,

                DateOfBirthText = me.DateOfBirth.ToString("yyyy-MM-dd"),
                GenderName = me.GenderName,

                CreatedAtText = me.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsActive = me.IsActive,

                Roles = roles
                    .Select(r => r.RoleName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList()
            };
        }

        public async Task UpdateEmailAsync(UpdateEmailVm vm, CancellationToken ct = default)
        {
            // Map VM -> API DTO
            var dto = new UpdateUserEmailDto { Email = vm.Email };
            await _api.UpdateMyEmailAsync(dto, ct);
        }

        public async Task ChangePasswordAsync(ChangePasswordVm vm, CancellationToken ct = default)
        {
            // Map VM -> API DTO
            var dto = new ChangePasswordDto
            {
                CurrentPassword = vm.CurrentPassword,
                NewPassword = vm.NewPassword
            };

            await _api.ChangeMyPasswordAsync(dto, ct);
        }
    }
}