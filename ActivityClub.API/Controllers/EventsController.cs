using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Services.Interfaces;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetEvents()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev is null) return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> CreateEvent(CreateEventDto dto)
        {
            var created = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetEvent), new { id = created.EventId }, created); //returns 201 + location header
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto dto)
        {
            var updated = await _eventService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
