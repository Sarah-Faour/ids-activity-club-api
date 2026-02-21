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
        {
            var lookups = await _lookupApiClient.GetByCodeAsync("Gender", ct);

            return lookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.SortOrder)
                .Select(l => new SelectListItem
                {
                    Value = l.LookupId.ToString(),
                    Text = l.Name
                })
                .ToList();
        }
    }
}