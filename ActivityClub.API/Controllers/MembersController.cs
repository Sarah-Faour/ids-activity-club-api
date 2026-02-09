using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembers()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        // GET: api/Members/5  (authenticated)
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



    }
}
