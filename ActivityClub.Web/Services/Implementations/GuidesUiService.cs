using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Guides;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class GuidesUiService : IGuidesUiService
    {
        private readonly IGuideApiClient _api;

        public GuidesUiService(IGuideApiClient api)
        {
            _api = api;
        }

        public async Task<List<GuideListItemVm>> GetListAsync(CancellationToken ct = default)
        {
            // 1) Call API -> returns List<GuideResponseDto>
            var dtos = await _api.GetAllAsync(ct);

            // 2) Map DTOs -> ViewModels (shape used by the UI)
            return dtos
                .Select(g => new GuideListItemVm
                {
                    GuideId = g.GuideId,
                    FullName = g.FullName,
                    JoiningDateText = g.JoiningDate.ToString("yyyy-MM-dd"),
                    ProfessionName = g.ProfessionName,
                    Photo = g.Photo,
                    IsActive = g.IsActive
                })
                .ToList();
        }

        public async Task<GuideDetailsVm?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            // 1) Call API -> returns GuideResponseDto? (null if not found)
            var g = await _api.GetByIdAsync(id, ct);
            if (g is null) return null;

            // 2) Map DTO -> Details VM
            return new GuideDetailsVm
            {
                GuideId = g.GuideId,
                UserId = g.UserId,
                FullName = g.FullName,
                JoiningDateText = g.JoiningDate.ToString("yyyy-MM-dd"),
                ProfessionId = g.ProfessionId,
                ProfessionName = g.ProfessionName,
                Photo = g.Photo,
                IsActive = g.IsActive
            };
        }
    }
}