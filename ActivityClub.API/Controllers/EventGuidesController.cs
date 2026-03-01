using ActivityClub.Contracts.DTOs.EventGuides;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/guides")]
    public class EventGuidesController : ControllerBase
    {
        private readonly IEventGuideService _service;

        public EventGuidesController(IEventGuideService service)
        {
            _service = service;
        }

        // GET: /api/events/{eventId}/guides  
        [AllowAnonymous] //anyone can view the guides of a specific event(public read, admin-only write)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventGuideResponseDto>>> GetGuidesForEvent([FromRoute] int eventId)
        {
            var guides = await _service.GetGuidesForEventAsync(eventId);

            if (guides is null)
                //return NotFound("Event not found or inactive.");
                return NotFound(new { message = "Event not found or inactive." });

            return Ok(guides);
        }

        // POST: /api/events/{eventId}/guides  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EventGuideResponseDto>> AssignGuideToEvent([FromRoute] int eventId, [FromBody] AssignGuideDto dto)
        {
            var created = await _service.AssignGuideToEventAsync(eventId, dto);
            return CreatedAtAction(nameof(GetGuidesForEvent), new { eventId }, created);
        }

        // DELETE: /api/events/{eventId}/guides/{guideId}  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{guideId:int}")]
        public async Task<IActionResult> UnassignGuideFromEvent([FromRoute] int eventId, [FromRoute] int guideId)
        {
            var removed = await _service.UnassignGuideFromEventAsync(eventId, guideId);

            if (!removed)
                //return NotFound("Assignment not found.");
                return NotFound(new { message = "Assignment not found." });

            return NoContent();
        }

        // GET: /api/events/{eventId}/guides/admin (ADMIN only) -> returns assignments even if guide inactive
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<EventGuideAdminResponseDto>>> GetGuidesForEventForAdmin(int eventId)
        {
            var guides = await _service.GetGuidesForEventForAdminAsync(eventId);
            if (guides is null) return NotFound(new { message = "Event not found." });
            return Ok(guides);
        }
    }
}
