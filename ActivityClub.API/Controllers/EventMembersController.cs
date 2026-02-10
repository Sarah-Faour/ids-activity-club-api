using ActivityClub.Contracts.DTOs.EventMembers;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/members")]
    [Authorize(Roles = "Admin")] // Admin-only for all endpoints (authenticated and authorized at the same time)
    public class EventMembersController : ControllerBase
    {
        private readonly IEventMemberService _eventMemberService;

        public EventMembersController(IEventMemberService eventMemberService)
        {
            _eventMemberService = eventMemberService;
        }

        // (Admin-only — enforce with JWT)
        // [Authorize(Roles = "Admin")]

        // GET: /api/events/{eventId}/members (authenticated)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventMemberResponseDto>>> GetMembersForEvent([FromRoute] int eventId)
        {
            var members = await _eventMemberService.GetMembersForEventAsync(eventId);
            return Ok(members);
        }

        // POST: /api/events/{eventId}/members/{memberId}  (ADMIN only)
        [HttpPost("{memberId:int}")]
        public async Task<IActionResult> AssignMemberToEvent([FromRoute] int eventId, [FromRoute] int memberId)
        {
            await _eventMemberService.AssignMemberToEventAsync(eventId, memberId);

            // 201 + Location header -> GET members of this event
            return CreatedAtAction(nameof(GetMembersForEvent), new { eventId }, null);
        }

        // DELETE: /api/events/{eventId}/members/{memberId}  (ADMIN only)
        [HttpDelete("{memberId:int}")]
        public async Task<IActionResult> UnassignMemberFromEvent([FromRoute] int eventId, [FromRoute] int memberId)
        {
            var deleted = await _eventMemberService.UnassignMemberFromEventAsync(eventId, memberId);

            if (!deleted)
                return NotFound(new { message = "Assignment not found." });

            return NoContent();
        }
    }
}
