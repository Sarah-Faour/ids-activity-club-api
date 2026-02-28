using ActivityClub.Contracts.Constants;
using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IGenericRepository<Event> _eventRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;
        private readonly IMapper _mapper;

       
        public EventService(
            IGenericRepository<Event> eventRepo,
            IGenericRepository<Lookup> lookupRepo,
            IMapper mapper)
        {
            _eventRepo = eventRepo;
            _lookupRepo = lookupRepo;
            _mapper = mapper;
        }

        public async Task<List<EventResponseDto>> GetAllAsync()
        {
            return await _eventRepo.Query()
                .Where(e => e.IsActive)
                .OrderBy(e => e.EventId)
                .ProjectTo<EventResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            return await _eventRepo.Query()
                .Where(e => e.EventId == id && e.IsActive)
                .ProjectTo<EventResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<EventResponseDto> CreateAsync(CreateEventDto dto)
        {
            ValidateBusinessRules(dto.DateFrom, dto.DateTo, dto.Cost);
            await ValidateLookups(dto.CategoryId, dto.StatusId);

            var nameKey = (dto.Name ?? string.Empty).Trim().ToUpper(); //Normalizing
            var destKey = (dto.Destination ?? string.Empty).Trim().ToUpper();

            // Check if an event with same signature already exists (active or inactive)
            var existing = await _eventRepo.Query()
                .FirstOrDefaultAsync(e =>
                    ((e.Name ?? "").Trim().ToUpper()) == nameKey && //Normalizing without creating a custom method that is written down in the code since when querying the database EF can't don't know how to translate a custom method to SQL, so it throws an InvalidOperationException, EF can only translate built in functions to SQL like .Trim() and .ToUpper()
                    e.DateFrom == dto.DateFrom &&
                    ((e.Destination ?? "").Trim().ToUpper()) == destKey);

            if (existing != null)
            {
                if (existing.IsActive)
                    throw new InvalidOperationException("An event with the same Name, DateFrom, and Destination already exists.");

                // Reactivate + update details from dto
                //first mapping then apply the buissness rule explicitly, otherwise if we update DTO to set IsActive it will override the busiiness rule that is set explicitly, so it will be reduntant and may lead to wrong data if accidently set to false.
                _mapper.Map(dto, existing);
                existing.IsActive = true;

                // Keep CreatedAt as-is (original creation moment), do NOT reset it.
                await _eventRepo.SaveChangesAsync();

                return await _eventRepo.Query()
                    .Where(e => e.EventId == existing.EventId)
                    .ProjectTo<EventResponseDto>(_mapper.ConfigurationProvider)
                    .FirstAsync();
            }

            var ev = _mapper.Map<Event>(dto);
            ev.CreatedAt = DateTime.UtcNow;
            ev.IsActive = true; //setting them explicitly 

            await _eventRepo.AddAsync(ev);
            await _eventRepo.SaveChangesAsync();

            return await _eventRepo.Query()
                .Where(e => e.EventId == ev.EventId)
                .ProjectTo<EventResponseDto>(_mapper.ConfigurationProvider)
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

            var nameKey = (dto.Name ?? string.Empty).Trim().ToUpper();
            var destKey = (dto.Destination ?? string.Empty).Trim().ToUpper();

            // Prevent updating into a duplicate of another active event
            var duplicateExists = await _eventRepo.Query().AnyAsync(e =>
                e.EventId != id &&
                e.IsActive &&
                ((e.Name ?? "").Trim().ToUpper()) == nameKey &&
                e.DateFrom == dto.DateFrom &&
                ((e.Destination ?? "").Trim().ToUpper()) == destKey);

            if (duplicateExists)
                throw new InvalidOperationException("Another active event already has the same Name, DateFrom, and Destination.");

            _mapper.Map(dto, ev); //mapping to an event that is already tracked in memory

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

        //For Admin-only
        public async Task<List<EventResponseDto>> GetAllForAdminAsync()
        {
            return await _eventRepo.Query()
                .OrderBy(e => e.EventId)
                .ProjectTo<EventResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> ReactivateAsync(int id)
        {
            var ev = await _eventRepo.Query().FirstOrDefaultAsync(e => e.EventId == id && !e.IsActive);
            if (ev is null) return false;

            ev.IsActive = true;
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
                throw new ArgumentException("Invalid CategoryId (must be an active activity category lookup).");

            var statusOk = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == statusId &&
                l.IsActive &&
                l.Code == LookupCodes.EventStatus);

            if (!statusOk)
                throw new ArgumentException("Invalid StatusId (must be an active event status lookup).");
        }

        /*
        private static string Normalize(string? value)
            => (value ?? string.Empty).Trim().ToUpper(); //it is a method of one expression (one line) so "=>" is used instead of "{ }"
        */

    }
}
