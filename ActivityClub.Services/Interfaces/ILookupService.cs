using ActivityClub.Contracts.DTOs.Lookups;

namespace ActivityClub.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<LookupResponseDto>> GetAllAsync();
        Task<LookupResponseDto?> GetByIdAsync(int id);
        Task<List<LookupResponseDto>> GetByCodeAsync(string code);
    }
}

