using ActivityClub.API.DTOs.Members;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public MembersController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetMembers()
        {
            var members = await _context.Members
                .Include(m => m.Profession)
                .Include(m => m.Nationality)
                .Where(m =>m.IsActive && m.User.IsActive)
                .Select(m => new MemberResponseDto
                {
                    MemberId = m.MemberId,
                    UserId = m.UserId,
                    FullName = m.FullName,
                    MobileNumber = m.MobileNumber,
                    EmergencyNumber = m.EmergencyNumber,
                    JoiningDate = m.JoiningDate,
                    Photo = m.Photo,
                    ProfessionId = m.ProfessionId,
                    ProfessionName = m.Profession != null ? m.Profession.Name : null,
                    NationalityId = m.NationalityId,
                    NationalityName = m.Nationality != null ? m.Nationality.Name : null,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(members);
        }

        // GET: api/Members/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberResponseDto>> GetMember(int id)
        {
            var member = await _context.Members
                .Include(m => m.Profession)
                .Include(m => m.Nationality)
                .Where(m => m.MemberId == id && m.IsActive && m.User.IsActive)
                .Select(m => new MemberResponseDto
                {
                    MemberId = m.MemberId,
                    UserId = m.UserId,
                    FullName = m.FullName,
                    MobileNumber = m.MobileNumber,
                    EmergencyNumber = m.EmergencyNumber,
                    JoiningDate = m.JoiningDate,
                    Photo = m.Photo,
                    ProfessionId = m.ProfessionId,
                    ProfessionName = m.Profession != null ? m.Profession.Name : null,
                    NationalityId = m.NationalityId,
                    NationalityName = m.Nationality != null ? m.Nationality.Name : null,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            if (member == null)
                return NotFound();

            return Ok(member);
        }

        [HttpPost]
        public async Task<ActionResult<MemberResponseDto>> CreateMember(CreateMemberDto dto)
        {
            // user must exist + be active
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.IsActive);
            if (user is null)
                return BadRequest("Invalid UserId (user not found or inactive).");

            // user must not already have a member record
            var existingMember = await _context.Members
                 .Include(m => m.Profession)
                 .Include(m => m.Nationality)
                 .FirstOrDefaultAsync(m => m.UserId == dto.UserId);

            if (existingMember != null)
            {
                if (existingMember.IsActive)
                    return Conflict("This user is already a member.");


                existingMember.IsActive = true;
                await _context.SaveChangesAsync();

                var activatedResponse = new MemberResponseDto
                {
                    MemberId = existingMember.MemberId,
                    UserId = existingMember.UserId,
                    FullName = existingMember.FullName,
                    MobileNumber = existingMember.MobileNumber,
                    EmergencyNumber = existingMember.EmergencyNumber,
                    JoiningDate = existingMember.JoiningDate,
                    Photo = existingMember.Photo,
                    ProfessionId = existingMember.ProfessionId,
                    ProfessionName = existingMember.Profession != null ? existingMember.Profession.Name : null,
                    NationalityId = existingMember.NationalityId,
                    NationalityName = existingMember.Nationality != null ? existingMember.Nationality.Name : null,
                    IsActive = existingMember.IsActive
                };

                return Ok(activatedResponse); // 200 — reactivated
            }


            // validate lookups (optional fields)
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.ProfessionId.Value && l.IsActive);
                if (!profExists) return BadRequest("Invalid ProfessionId.");
            }

            if (dto.NationalityId.HasValue)
            {
                var natExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.NationalityId.Value && l.IsActive);
                if (!natExists) return BadRequest("Invalid NationalityId.");
            }

            var member = new Member
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                MobileNumber = dto.MobileNumber,
                EmergencyNumber = dto.EmergencyNumber,
                JoiningDate = dto.JoiningDate,
                Photo = dto.Photo,
                ProfessionId = dto.ProfessionId,
                NationalityId = dto.NationalityId,
                IsActive = true // ✅ explicit (better for Murex)
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // load related lookup names for response
            await _context.Entry(member).Reference(m => m.Profession).LoadAsync();
            await _context.Entry(member).Reference(m => m.Nationality).LoadAsync();

            var response = new MemberResponseDto
            {
                MemberId = member.MemberId,
                UserId = member.UserId,
                FullName = member.FullName,
                MobileNumber = member.MobileNumber,
                EmergencyNumber = member.EmergencyNumber,
                JoiningDate = member.JoiningDate,
                Photo = member.Photo,
                ProfessionId = member.ProfessionId,
                ProfessionName = member.Profession != null ? member.Profession.Name : null,
                NationalityId = member.NationalityId,
                NationalityName = member.Nationality?.Name,
                IsActive = member.IsActive
            };

            return CreatedAtAction(nameof(GetMember), new { id = member.MemberId }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMember(int id, UpdateMemberDto dto)
        {
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id && m.IsActive && m.User.IsActive);

            if (member is null)
                return NotFound();

            // validate lookups (optional fields)
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.ProfessionId.Value && l.IsActive);
                if (!profExists) return BadRequest("Invalid ProfessionId.");
            }

            if (dto.NationalityId.HasValue)
            {
                var natExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.NationalityId.Value && l.IsActive);
                if (!natExists) return BadRequest("Invalid NationalityId.");
            }

            // (not required — better for Murex) don't allow changing UserId in update
            member.FullName = dto.FullName;
            member.MobileNumber = dto.MobileNumber;
            member.EmergencyNumber = dto.EmergencyNumber;
            member.JoiningDate = dto.JoiningDate;
            member.Photo = dto.Photo;
            member.ProfessionId = dto.ProfessionId;
            member.NationalityId = dto.NationalityId;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // DELETE: api/members/{id}
        // Soft delete: deactivate the Member profile (User stays active for Guide/Admin)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            // Find member 
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id && m.IsActive && m.User.IsActive);

            if (member == null)
                return NotFound();

            // Soft delete: deactivate the member
            member.IsActive = false;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }



    }
}
