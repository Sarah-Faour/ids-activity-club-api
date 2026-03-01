using ActivityClub.Contracts.DTOs.EventGuides;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class EventGuideService : IEventGuideService
    {
        private readonly IGenericRepository<EventGuide> _eventGuideRepo;
        private readonly IGenericRepository<Event> _eventRepo;
        private readonly IGenericRepository<Guide> _guideRepo;

        public EventGuideService(
            IGenericRepository<EventGuide> eventGuideRepo,
            IGenericRepository<Event> eventRepo,
            IGenericRepository<Guide> guideRepo)
        {
            _eventGuideRepo = eventGuideRepo;
            _eventRepo = eventRepo;
            _guideRepo = guideRepo;
        }

        // GET: /api/events/{eventId}/guides
        public async Task<List<EventGuideResponseDto>?> GetGuidesForEventAsync(int eventId)
        {
            var eventExists = await _eventRepo.Query()
                .AnyAsync(e => e.EventId == eventId && e.IsActive);

            if (!eventExists)
                return null;

            return await _eventGuideRepo.Query()
                .Where(eg =>
                    eg.EventId == eventId &&
                    eg.Event.IsActive &&
                    eg.Guide.IsActive &&
                    eg.Guide.User.IsActive)
                .Select(eg => new EventGuideResponseDto
                {
                    EventGuideId = eg.EventGuideId,
                    EventId = eg.EventId,
                    GuideId = eg.GuideId,
                    GuideName = eg.Guide.FullName,
                    AssignedDate = eg.AssignedDate
                })
                .ToListAsync();
        }

        // POST: /api/events/{eventId}/guides
        public async Task<EventGuideResponseDto> AssignGuideToEventAsync(int eventId, AssignGuideDto dto)
        {
            var eventExists = await _eventRepo.Query()
                .AnyAsync(e => e.EventId == eventId && e.IsActive);

            if (!eventExists)
                throw new ArgumentException("Event not found or inactive.");

            var guideExists = await _guideRepo.Query()
                .AnyAsync(g =>
                    g.GuideId == dto.GuideId &&
                    g.IsActive &&
                    g.User.IsActive);

            if (!guideExists)
                throw new ArgumentException("Invalid GuideId (guide not found or inactive).");

            var alreadyAssigned = await _eventGuideRepo.Query()
                .AnyAsync(eg => eg.EventId == eventId && eg.GuideId == dto.GuideId);

            if (alreadyAssigned)
                throw new InvalidOperationException("This guide is already assigned to this event.");

            var link = new EventGuide
            {
                EventId = eventId,
                GuideId = dto.GuideId,
                AssignedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _eventGuideRepo.AddAsync(link);
            await _eventGuideRepo.SaveChangesAsync();

            // Load Guide for GuideName (uses the repo helper you already added ✅)
            await _eventGuideRepo.LoadReferenceAsync(link, x => x.Guide);

            return new EventGuideResponseDto
            {
                EventGuideId = link.EventGuideId,
                EventId = link.EventId,
                GuideId = link.GuideId,
                GuideName = link.Guide?.FullName ?? string.Empty,
                AssignedDate = link.AssignedDate
            };
        }

        // DELETE: /api/events/{eventId}/guides/{guideId}
        public async Task<bool> UnassignGuideFromEventAsync(int eventId, int guideId)
        {
            var link = await _eventGuideRepo.Query()
                .FirstOrDefaultAsync(eg => eg.EventId == eventId && eg.GuideId == guideId);

            if (link is null)
                return false;

            _eventGuideRepo.Remove(link);
            await _eventGuideRepo.SaveChangesAsync();
            return true;
        }

        //Getting all assigned Guides(Active and Inactive)
        public async Task<List<EventGuideAdminResponseDto>?> GetGuidesForEventForAdminAsync(int eventId)
        {
            var exists = await _eventRepo.Query().AnyAsync(e => e.EventId == eventId);
            if (!exists) return null;

            return await _eventGuideRepo.Query()
                .Where(eg => eg.EventId == eventId)
                .Select(eg => new EventGuideAdminResponseDto
                {
                    EventGuideId = eg.EventGuideId,
                    EventId = eg.EventId,
                    EventIsActive = eg.Event.IsActive,

                    GuideId = eg.GuideId,
                    GuideName = eg.Guide.FullName,
                    GuideIsActive = eg.Guide.IsActive,
                    GuideUserIsActive = eg.Guide.User.IsActive,

                    AssignedDate = eg.AssignedDate
                })
                .ToListAsync();
        }
    }
}

