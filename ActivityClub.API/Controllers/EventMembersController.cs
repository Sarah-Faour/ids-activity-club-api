using ActivityClub.Contracts.DTOs.EventMembers;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/members")]
    public class EventMembersController : ControllerBase
    {
        private readonly IEventMemberService _eventMemberService;

        public EventMembersController(IEventMemberService eventMemberService)
        {
            _eventMemberService = eventMemberService;
        }

        // (Admin-only — enforce later with JWT)
        // [Authorize(Roles = "Admin")]

        // GET: /api/events/{eventId}/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventMemberResponseDto>>> GetMembersForEvent(int eventId)
        {
            var members = await _eventMemberService.GetMembersForEventAsync(eventId);
            return Ok(members);
        }

        // POST: /api/events/{eventId}/members/{memberId}
        [HttpPost("{memberId:int}")]
        public async Task<IActionResult> AssignMemberToEvent(int eventId, int memberId)
        {
            await _eventMemberService.AssignMemberToEventAsync(eventId, memberId);

            // 201 + Location header -> GET members of this event
            return CreatedAtAction(nameof(GetMembersForEvent), new { eventId }, null);
        }

        // DELETE: /api/events/{eventId}/members/{memberId}
        [HttpDelete("{memberId:int}")]
        public async Task<IActionResult> UnassignMemberFromEvent(int eventId, int memberId)
        {
            var deleted = await _eventMemberService.UnassignMemberFromEventAsync(eventId, memberId);

            if (!deleted)
                return NotFound("Assignment not found.");

            return NoContent();
        }
    }
}
