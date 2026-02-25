using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface ILookupUiService
    {
        Task<IReadOnlyList<SelectListItem>> GetGenderOptionsAsync(CancellationToken ct = default);

        Task<IReadOnlyList<SelectListItem>> GetProfessionOptionsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<SelectListItem>> GetNationalityOptionsAsync(CancellationToken ct = default);
    }
}