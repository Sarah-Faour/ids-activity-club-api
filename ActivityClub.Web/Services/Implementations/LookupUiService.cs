using ActivityClub.Contracts.Constants;
using ActivityClub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class LookupUiService : ILookupUiService
    {
        private readonly ILookupApiClient _lookupApiClient;

        public LookupUiService(ILookupApiClient lookupApiClient)
        {
            _lookupApiClient = lookupApiClient;
        }

        public async Task<IReadOnlyList<SelectListItem>> GetGenderOptionsAsync(CancellationToken ct = default)
            => await GetOptionsAsync(LookupCodes.Gender, ct);

        public async Task<IReadOnlyList<SelectListItem>> GetProfessionOptionsAsync(CancellationToken ct = default)
            => await GetOptionsAsync(LookupCodes.Profession, ct);

        public async Task<IReadOnlyList<SelectListItem>> GetNationalityOptionsAsync(CancellationToken ct = default)
            => await GetOptionsAsync(LookupCodes.Nationality, ct);
        
        public async Task<IReadOnlyList<SelectListItem>> GetEventCategoryOptionsAsync(CancellationToken ct = default)
            => await GetOptionsAsync(LookupCodes.ActivityCategory, ct);

        public async Task<IReadOnlyList<SelectListItem>> GetEventStatusOptionsAsync(CancellationToken ct = default)
            => await GetOptionsAsync(LookupCodes.EventStatus, ct);

        private async Task<IReadOnlyList<SelectListItem>> GetOptionsAsync(string code, CancellationToken ct)
        {
            var lookups = await _lookupApiClient.GetByCodeAsync(code, ct);

            var items = lookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.SortOrder)
                .Select(l => new SelectListItem
                {
                    Value = l.LookupId.ToString(),
                    Text = l.Name
                })
                .ToList();

            // Optional "no selection" (good UX when the field is nullable)
            items.Insert(0, new SelectListItem { Value = "", Text = "-- Select --" });

            return items;
        }
    }
}