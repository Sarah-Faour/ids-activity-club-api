using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/events (authenticated)
        [AllowAnonymous] //no authentication (log in) is needed to browse Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetEvents()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        // GET: api/events/5 (authenticated)
        [AllowAnonymous] 
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev is null) return NotFound();
            return Ok(ev);
        }

        // POST: api/events (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> CreateEvent([FromBody] CreateEventDto dto)
        {
            var created = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetEvent), new { id = created.EventId }, created); //returns 201 + location header
        }

        // PUT: api/events/5 (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            var updated = await _eventService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/events/5 (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
