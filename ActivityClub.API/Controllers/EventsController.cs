using ActivityClub.API.DTOs.Events;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public EventsController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // GET: /api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Where(e => e.IsActive == true)
                .Include(e => e.Category)
                .Include(e => e.Status)
                .Select(e => new EventResponseDto
                {
                    EventId = e.EventId,
                    Name = e.Name,
                    Description = e.Description,
                    Destination = e.Destination,
                    DateFrom = e.DateFrom,
                    DateTo = e.DateTo,
                    Cost = e.Cost,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.Name, // adjust if Lookup uses Name vs Description
                    StatusId = e.StatusId,
                    StatusName = e.Status.Name,     // adjust if Lookup uses Name vs Description
                    CreatedAt = e.CreatedAt,
                    IsActive = e.IsActive
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: /api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
        {
            var ev = await _context.Events
                .AsNoTracking()
                .Include(e => e.Category)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(e => e.EventId == id && e.IsActive == true);

            if (ev == null)
            {
                return NotFound();
            }

            var dto = new EventResponseDto
            {
                EventId = ev.EventId,
                Name = ev.Name,
                Description = ev.Description,
                Destination = ev.Destination,
                DateFrom = ev.DateFrom,
                DateTo = ev.DateTo,
                Cost = ev.Cost,
                CategoryId = ev.CategoryId,
                CategoryName = ev.Category.Name,
                StatusId = ev.StatusId,
                StatusName = ev.Status.Name,
                CreatedAt = ev.CreatedAt,
                IsActive = ev.IsActive
            };

            return Ok(dto);
        }

        // POST: /api/events
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> CreateEvent([FromBody] CreateEventDto dto)
        {
            // Basic business validation
            if (dto.DateTo < dto.DateFrom)
                return BadRequest("DateTo cannot be earlier than DateFrom.");

            if (dto.Cost < 0)
                return BadRequest("Cost cannot be negative.");

            // Validate lookup IDs exist (Category + Status)
            var categoryExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest("Invalid CategoryId.");

            var statusExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.StatusId);
            if (!statusExists)
                return BadRequest("Invalid StatusId.");

            // Map DTO -> Entity
            var ev = new ActivityClub.Data.Models.Event
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

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            // Load lookups so we can return CategoryName/StatusName in the response
            await _context.Entry(ev).Reference(x => x.Category).LoadAsync();
            await _context.Entry(ev).Reference(x => x.Status).LoadAsync();

            var response = new EventResponseDto
            {
                EventId = ev.EventId,
                Name = ev.Name,
                Description = ev.Description,
                Destination = ev.Destination,
                DateFrom = ev.DateFrom,
                DateTo = ev.DateTo,
                Cost = ev.Cost,
                CategoryId = ev.CategoryId,
                CategoryName = ev.Category.Name,
                StatusId = ev.StatusId,
                StatusName = ev.Status.Name,
                CreatedAt = ev.CreatedAt,
                IsActive = ev.IsActive
            };

            // 201 Created + Location header pointing to GET /api/events/{id}
            return CreatedAtAction(nameof(GetEvent), new { id = ev.EventId }, response);
        }

        // PUT: /api/Events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            // Basic validation
            if (dto.DateTo < dto.DateFrom)
                return BadRequest("DateTo cannot be earlier than DateFrom.");

            if (dto.Cost < 0)
                return BadRequest("Cost cannot be negative.");

            // Find existing event
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);
            if (ev == null)
                return NotFound();

            // Validate lookup IDs exist
            var categoryExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest("Invalid CategoryId.");

            var statusExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.StatusId);
            if (!statusExists)
                return BadRequest("Invalid StatusId.");

            // Update fields
            ev.Name = dto.Name;
            ev.Description = dto.Description;
            ev.CategoryId = dto.CategoryId;
            ev.Destination = dto.Destination;
            ev.DateFrom = dto.DateFrom;
            ev.DateTo = dto.DateTo;
            ev.Cost = dto.Cost;
            ev.StatusId = dto.StatusId;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // DELETE: api/Events/{id}
        // Soft delete: sets IsActive = false (not required by assignment — better for Murex)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound();

            // Soft delete
            ev.IsActive = false;

            await _context.SaveChangesAsync();
            return NoContent(); // 204
        }


    }
}

