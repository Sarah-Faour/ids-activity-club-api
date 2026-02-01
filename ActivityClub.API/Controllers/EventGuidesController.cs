using ActivityClub.Contracts.DTOs.EventGuides;
using ActivityClub.Services.Interfaces;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventGuideResponseDto>>> GetGuidesForEvent(int eventId)
        {
            var guides = await _service.GetGuidesForEventAsync(eventId);

            if (guides is null)
                return NotFound("Event not found or inactive.");

            return Ok(guides);
        }

        // POST: /api/events/{eventId}/guides
        [HttpPost]
        public async Task<ActionResult<EventGuideResponseDto>> AssignGuideToEvent(int eventId, AssignGuideDto dto)
        {
            var created = await _service.AssignGuideToEventAsync(eventId, dto);
            return CreatedAtAction(nameof(GetGuidesForEvent), new { eventId }, created);
        }

        // DELETE: /api/events/{eventId}/guides/{guideId}
        [HttpDelete("{guideId:int}")]
        public async Task<IActionResult> UnassignGuideFromEvent(int eventId, int guideId)
        {
            var removed = await _service.UnassignGuideFromEventAsync(eventId, guideId);

            if (!removed)
                return NotFound("Assignment not found.");

            return NoContent();
        }
    }
}
