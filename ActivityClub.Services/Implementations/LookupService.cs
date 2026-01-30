using System.Linq.Expressions;
using ActivityClub.Contracts.DTOs.Lookups;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class LookupService : ILookupService
    {
        private readonly IGenericRepository<Lookup> _lookupRepo;

        private static readonly Expression<Func<Lookup, LookupResponseDto>> LookupSelect =
            l => new LookupResponseDto
            {
                LookupId = l.LookupId,
                Code = l.Code,
                Name = l.Name,
                SortOrder = l.SortOrder,
                IsActive = l.IsActive
            };

        public LookupService(IGenericRepository<Lookup> lookupRepo)
        {
            _lookupRepo = lookupRepo;
        }

        public async Task<List<LookupResponseDto>> GetAllAsync()
        {
            return await _lookupRepo.Query()
                .Where(l => l.IsActive)
                .OrderBy(l => l.Code)
                .ThenBy(l => l.SortOrder)
                .Select(LookupSelect)
                .ToListAsync();
        }

        public async Task<LookupResponseDto?> GetByIdAsync(int id)
        {
            return await _lookupRepo.Query()
                .Where(l => l.LookupId == id && l.IsActive)
                .Select(LookupSelect)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LookupResponseDto>> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code is required.");

            // Normalize to match DB style (your DB uses uppercase codes)
            var normalized = code.Trim().ToUpperInvariant();

            return await _lookupRepo.Query()
                .Where(l => l.IsActive && l.Code == normalized)
                .OrderBy(l => l.SortOrder)
                .ThenBy(l => l.Name)
                .Select(LookupSelect)
                .ToListAsync();
        }
    }
}
