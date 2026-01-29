using System.Linq.Expressions;
using ActivityClub.Contracts.Constants;
using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class GuideService : IGuideService
    {
        private readonly IGenericRepository<Guide> _guideRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;

        // Projection: returns GuideResponseDto directly from SQL (no Include needed)
        private static readonly Expression<Func<Guide, GuideResponseDto>> GuideSelect =
            g => new GuideResponseDto
            {
                GuideId = g.GuideId,
                UserId = g.UserId,
                FullName = g.FullName,
                JoiningDate = g.JoiningDate,
                Photo = g.Photo,
                ProfessionId = g.ProfessionId,
                ProfessionName = g.Profession != null ? g.Profession.Name : null,
                IsActive = g.IsActive
            };

        public GuideService(
            IGenericRepository<Guide> guideRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<Lookup> lookupRepo)
        {
            _guideRepo = guideRepo;
            _userRepo = userRepo;
            _lookupRepo = lookupRepo;
        }

        public async Task<List<GuideResponseDto>> GetAllAsync()
        {
            return await _guideRepo.Query()
                .Where(g => g.IsActive && g.User.IsActive)
                .OrderBy(g => g.GuideId)
                .Select(GuideSelect)
                .ToListAsync();
        }

        public async Task<GuideResponseDto?> GetByIdAsync(int id)
        {
            return await _guideRepo.Query()
                .Where(g => g.GuideId == id && g.IsActive && g.User.IsActive)
                .Select(GuideSelect)
                .FirstOrDefaultAsync();
        }

        public async Task<GuideResponseDto> CreateAsync(CreateGuideDto dto)
        {
            // user must exist + be active
            var userExists = await _userRepo.Query()
                .AnyAsync(u => u.UserId == dto.UserId && u.IsActive);

            if (!userExists)
                throw new ArgumentException("Invalid UserId (user not found or inactive).");

            // validate profession (optional) + must be code PROFESSION
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _lookupRepo.Query().AnyAsync(l =>
                    l.LookupId == dto.ProfessionId.Value &&
                    l.IsActive &&
                    l.Code == LookupCodes.Profession);

                if (!profExists)
                    throw new ArgumentException("Invalid ProfessionId.");
            }

            // existing guide row for same user?
            var existing = await _guideRepo.Query()
                .FirstOrDefaultAsync(g => g.UserId == dto.UserId);

            if (existing != null)
            {
                if (existing.IsActive)
                    throw new InvalidOperationException("This user is already a guide.");

                // Reactivate only
                existing.IsActive = true;
                await _guideRepo.SaveChangesAsync();

                // return using projection
                return await _guideRepo.Query()
                    .Where(g => g.GuideId == existing.GuideId)
                    .Select(GuideSelect)
                    .FirstAsync();
            }

            var guide = new Guide
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                JoiningDate = dto.JoiningDate,
                Photo = dto.Photo,
                ProfessionId = dto.ProfessionId,
                IsActive = true
            };

            await _guideRepo.AddAsync(guide);
            await _guideRepo.SaveChangesAsync();

            return await _guideRepo.Query()
                .Where(g => g.GuideId == guide.GuideId)
                .Select(GuideSelect)
                .FirstAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateGuideDto dto)
        {
            var guide = await _guideRepo.Query()
                .FirstOrDefaultAsync(g => g.GuideId == id && g.IsActive && g.User.IsActive);

            if (guide is null)
                return false;

            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _lookupRepo.Query().AnyAsync(l =>
                    l.LookupId == dto.ProfessionId.Value &&
                    l.IsActive &&
                    l.Code == LookupCodes.Profession);

                if (!profExists)
                    throw new ArgumentException("Invalid ProfessionId.");
            }

            guide.FullName = dto.FullName;
            guide.JoiningDate = dto.JoiningDate;
            guide.Photo = dto.Photo;
            guide.ProfessionId = dto.ProfessionId;

            await _guideRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var guide = await _guideRepo.Query()
                .FirstOrDefaultAsync(g => g.GuideId == id && g.IsActive && g.User.IsActive);

            if (guide is null)
                return false;

            guide.IsActive = false;
            await _guideRepo.SaveChangesAsync();
            return true;
        }
    }
}
