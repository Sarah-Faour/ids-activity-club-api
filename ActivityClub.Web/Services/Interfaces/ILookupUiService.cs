using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface ILookupUiService
    {
        Task<IReadOnlyList<SelectListItem>> GetGenderOptionsAsync(CancellationToken ct = default);

        Task<IReadOnlyList<SelectListItem>> GetProfessionOptionsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<SelectListItem>> GetNationalityOptionsAsync(CancellationToken ct = default);

        // NEW for Admin Events
        Task<IReadOnlyList<SelectListItem>> GetEventCategoryOptionsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<SelectListItem>> GetEventStatusOptionsAsync(CancellationToken ct = default);
    }
}