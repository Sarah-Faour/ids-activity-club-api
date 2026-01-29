using System.Linq.Expressions;
using ActivityClub.Contracts.Constants;
using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IGenericRepository<Event> _eventRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;

        // Projection: returns EventResponseDto directly from SQL
        private static readonly Expression<Func<Event, EventResponseDto>> EventSelect =
            e => new EventResponseDto
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                Destination = e.Destination,
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                Cost = e.Cost,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                StatusId = e.StatusId,
                StatusName = e.Status.Name,
                CreatedAt = e.CreatedAt,
                IsActive = e.IsActive
            };

        public EventService(
            IGenericRepository<Event> eventRepo,
            IGenericRepository<Lookup> lookupRepo)
        {
            _eventRepo = eventRepo;
            _lookupRepo = lookupRepo;
        }

        public async Task<List<EventResponseDto>> GetAllAsync()
        {
            return await _eventRepo.Query()
                .Where(e => e.IsActive)
                .OrderBy(e => e.EventId)
                .Select(EventSelect)
                .ToListAsync();
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            return await _eventRepo.Query()
                .Where(e => e.EventId == id && e.IsActive)
                .Select(EventSelect)
                .FirstOrDefaultAsync();
        }

        public async Task<EventResponseDto> CreateAsync(CreateEventDto dto)
        {
            ValidateBusinessRules(dto.DateFrom, dto.DateTo, dto.Cost);
            await ValidateLookups(dto.CategoryId, dto.StatusId);

            var ev = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Destination = dto.Destination,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                Cost = dto.Cost,
                StatusId = dto.StatusId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _eventRepo.AddAsync(ev);
            await _eventRepo.SaveChangesAsync();

            return await _eventRepo.Query()
                .Where(e => e.EventId == ev.EventId)
                .Select(EventSelect)
                .FirstAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateEventDto dto)
        {
            ValidateBusinessRules(dto.DateFrom, dto.DateTo, dto.Cost);
            await ValidateLookups(dto.CategoryId, dto.StatusId);

            var ev = await _eventRepo.Query()
                .FirstOrDefaultAsync(e => e.EventId == id && e.IsActive);

            if (ev is null)
                return false;

            ev.Name = dto.Name;
            ev.Description = dto.Description;
            ev.CategoryId = dto.CategoryId;
            ev.Destination = dto.Destination;
            ev.DateFrom = dto.DateFrom;
            ev.DateTo = dto.DateTo;
            ev.Cost = dto.Cost;
            ev.StatusId = dto.StatusId;

            await _eventRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var ev = await _eventRepo.Query()
                .FirstOrDefaultAsync(e => e.EventId == id && e.IsActive);

            if (ev is null)
                return false;

            ev.IsActive = false;
            await _eventRepo.SaveChangesAsync();
            return true;
        }

        private static void ValidateBusinessRules(DateOnly dateFrom, DateOnly dateTo, decimal cost)
        {
            if (dateTo < dateFrom)
                throw new ArgumentException("DateTo cannot be earlier than DateFrom.");

            if (cost < 0)
                throw new ArgumentException("Cost cannot be negative.");
        }

        private async Task ValidateLookups(int categoryId, int statusId)
        {
            var categoryOk = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == categoryId &&
                l.IsActive &&
                l.Code == LookupCodes.ActivityCategory);

            if (!categoryOk)
                throw new ArgumentException("Invalid CategoryId.");

            var statusOk = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == statusId &&
                l.IsActive &&
                l.Code == LookupCodes.EventStatus);

            if (!statusOk)
                throw new ArgumentException("Invalid StatusId.");
        }
    }
}
