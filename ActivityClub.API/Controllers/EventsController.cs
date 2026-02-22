using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventMemberService _eventMemberService;

        public EventsController(IEventService eventService, IEventMemberService eventMemberService)
        {
            _eventService = eventService;
            _eventMemberService = eventMemberService;
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

        // POST: api/events/5/join  (authenticated member joins himself)
        [Authorize]
        [HttpPost("{id:int}/join")]
        public async Task<IActionResult> JoinEvent(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            await _eventMemberService.JoinEventAsync(id, userId.Value);
            return NoContent(); // 204 (simple success)
        }

        private int? GetCurrentUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            sub ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(sub, out var parsed) ? parsed : null;
        }
    }
}
