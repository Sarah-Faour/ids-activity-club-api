using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembers()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        // GET: api/Members/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberResponseDto>> GetMember(int id)
        {
            var member = await _memberService.GetByIdAsync(id);

            if (member is null)
                return NotFound();

            return Ok(member);

        }

        [HttpPost]
        public async Task<ActionResult<MemberResponseDto>> CreateMember(CreateMemberDto dto)
        {
            var created = await _memberService.CreateAsync(dto);

            // Keep the same REST behavior you had: 201 + Location header pointing to GET by id
            return CreatedAtAction(nameof(GetMember), new { id = created.MemberId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMember(int id, UpdateMemberDto dto)
        {
            var updated = await _memberService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent(); // 204
        }

        // DELETE: api/members/{id}
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
