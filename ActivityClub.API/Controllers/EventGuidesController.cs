using ActivityClub.API.DTOs.EventGuides;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/guides")]
    public class EventGuidesController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public EventGuidesController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // GET: /api/events/{eventId}/guides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventGuideResponseDto>>> GetGuidesForEvent(int eventId)
        {
            // Include NOT needed here (projection via Select) — we used Include before sometimes, but not required.
            var guides = await _context.EventGuides
                .Where(eg => eg.EventId == eventId && eg.Event.IsActive == true)
                .Select(eg => new EventGuideResponseDto
                {
                    EventGuideId = eg.EventGuideId,
                    EventId = eg.EventId,
                    GuideId = eg.GuideId,
                    GuideName = eg.Guide.FullName,
                    AssignedDate = eg.AssignedDate
                })
                .ToListAsync();

            return Ok(guides);
        }

        // POST: /api/events/{eventId}/guides
        [HttpPost]
        public async Task<ActionResult<EventGuideResponseDto>> AssignGuideToEvent(int eventId, AssignGuideDto dto)
        {
            // validate event exists + active
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId && e.IsActive == true);
            if (!eventExists)
                return NotFound("Event not found or inactive.");

            // validate guide exists + active + user active (better for Murex)
            var guideExists = await _context.Guides.AnyAsync(g =>
                g.GuideId == dto.GuideId &&
                g.IsActive == true &&
                g.User.IsActive == true);

            if (!guideExists)
                return BadRequest("Invalid GuideId (guide not found or inactive).");

            // prevent duplicates (unique constraint)
            var alreadyAssigned = await _context.EventGuides.AnyAsync(eg => eg.EventId == eventId && eg.GuideId == dto.GuideId);
            if (alreadyAssigned)
                return Conflict("This guide is already assigned to this event.");

            var link = new EventGuide
            {
                EventId = eventId,
                GuideId = dto.GuideId,
                AssignedDate = DateOnly.FromDateTime(DateTime.Today)
            };

            _context.EventGuides.Add(link);
            await _context.SaveChangesAsync();

            // load guide name for response (Include not used; explicit load is clear)
            await _context.Entry(link).Reference(x => x.Guide).LoadAsync();

            var response = new EventGuideResponseDto
            {
                EventGuideId = link.EventGuideId,
                EventId = link.EventId,
                GuideId = link.GuideId,
                GuideName = link.Guide.FullName,
                AssignedDate = link.AssignedDate
            };

            return CreatedAtAction(nameof(GetGuidesForEvent), new { eventId }, response);
        }

        // DELETE: /api/events/{eventId}/guides/{guideId}
        [HttpDelete("{guideId:int}")]
        public async Task<IActionResult> UnassignGuideFromEvent(int eventId, int guideId)
        {
            var link = await _context.EventGuides
                .FirstOrDefaultAsync(eg => eg.EventId == eventId && eg.GuideId == guideId);

            if (link == null)
                return NotFound();

            // hard delete (correct for join table)
            _context.EventGuides.Remove(link);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
