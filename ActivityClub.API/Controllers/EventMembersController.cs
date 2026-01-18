using ActivityClub.API.DTOs.EventMembers;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/members")]
    public class EventMembersController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public EventMembersController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // (Admin-only — enforce later with JWT)
        // [Authorize(Roles = "Admin")]

        // GET: /api/events/{eventId}/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventMemberResponseDto>>> GetMembersForEvent(int eventId)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId && e.IsActive);
            if (!eventExists)
                return NotFound("Event not found or inactive.");

            var members = await _context.EventMembers
                .Where(em => em.EventId == eventId
                             && em.Member.IsActive
                             && em.Member.User.IsActive)
                .Select(em => new EventMemberResponseDto
                {
                    EventMemberId = em.EventMemberId,
                    EventId = em.EventId,
                    MemberId = em.MemberId,
                    JoinDate = em.JoinDate,
                    MemberName = em.Member.FullName
                })
                .ToListAsync();

            return Ok(members);
        }

        // POST: /api/events/{eventId}/members/{memberId}
        [HttpPost("{memberId:int}")]
        public async Task<IActionResult> AssignMemberToEvent(int eventId, int memberId)
        {
            // Validate event
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId && e.IsActive );
            if (ev is null)
                return NotFound("Event not found or inactive.");

            // Validate member + user active
            var memberExists = await _context.Members.AnyAsync(m => m.MemberId == memberId && m.IsActive && m.User.IsActive);
            if (!memberExists)
                return BadRequest("Member not found or inactive.");

            // Prevent duplicates (nice API message before DB unique index throws)
            var exists = await _context.EventMembers.AnyAsync(em => em.EventId == eventId && em.MemberId == memberId);
            if (exists)
                return Conflict("Member is already assigned to this event.");

            var link = new EventMember
            {
                EventId = eventId,
                MemberId = memberId,
                JoinDate = DateOnly.FromDateTime(DateTime.UtcNow) // since your column is DateOnly
            };

            _context.EventMembers.Add(link);
            await _context.SaveChangesAsync();

            // 201 + Location header -> GET members of this event
            return CreatedAtAction(nameof(GetMembersForEvent), new { eventId }, null);
        }

        // DELETE: /api/events/{eventId}/members/{memberId}
        [HttpDelete("{memberId:int}")]
        public async Task<IActionResult> UnassignMemberFromEvent(int eventId, int memberId)
        {
            var link = await _context.EventMembers
                .FirstOrDefaultAsync(em => em.EventId == eventId && em.MemberId == memberId);

            if (link is null)
                return NotFound("Assignment not found.");

            _context.EventMembers.Remove(link);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
