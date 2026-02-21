using ActivityClub.Contracts.DTOs.Lookups;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface ILookupApiClient
    {
        Task<IReadOnlyList<LookupResponseDto>> GetByCodeAsync(string code, CancellationToken ct = default);
    }
}