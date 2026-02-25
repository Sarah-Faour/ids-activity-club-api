using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // secure-by-default: JWT required for all endpoints
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // GET: api/Members  (authenticated)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembers()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        // GET: api/Members/5  (authenticated)
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberResponseDto>> GetMember(int id)
        {
            var member = await _memberService.GetByIdAsync(id);

            if (member is null)
                return NotFound();

            return Ok(member);

        }

        // POST: api/members (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MemberResponseDto>> CreateMember([FromBody] CreateMemberDto dto)
        {
            var created = await _memberService.CreateAsync(dto);

            // Keep the same REST behavior you had: 201 + Location header pointing to GET by id
            return CreatedAtAction(nameof(GetMember), new { id = created.MemberId }, created);
        }

        // PUT: api/members/5 (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] UpdateMemberDto dto)
        {
            var updated = await _memberService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent(); // 204
        }

        // DELETE: api/members/{id}  (ADMIN only)
        [Authorize(Roles = "Admin")]
        // Soft delete: deactivate the Member profile (User stays active for Guide/Admin)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var deleted = await _memberService.SoftDeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent(); // 204
        }


        // ----------------------------
        // /me endpoints (self-service)
        // ----------------------------

        // GET: api/members/me
        [HttpGet("me")]
        public async Task<ActionResult<MemberResponseDto>> GetMyMemberProfile()
        {
            var myId = GetCurrentUserId();
            if (myId is null) return Unauthorized();

            var me = await _memberService.GetMyProfileAsync(myId.Value);
            if (me is null) return NotFound(new { message = "Member profile not found or inactive." });

            return Ok(me);
        }

        // PUT: api/members/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyMemberProfile([FromBody] UpdateMemberDto dto)
        {
            var myId = GetCurrentUserId();
            if (myId is null) return Unauthorized();

            var ok = await _memberService.UpdateMyProfileAsync(myId.Value, dto);
            if (!ok) return NotFound(new { message = "Member profile not found or inactive." });

            return NoContent();
        }

        // --------------------
        // Helper Methods
        // --------------------
        private int? GetCurrentUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            sub ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(sub, out var id) ? id : null;
        }


    }
}
