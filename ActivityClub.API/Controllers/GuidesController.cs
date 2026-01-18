using ActivityClub.API.DTOs.Guides;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuidesController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public GuidesController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // GET: api/Guides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuideResponseDto>>> GetGuides()
        {
            var guides = await _context.Guides
                .Include(g => g.Profession) // needed for ProfessionName
                .Where(g => g.IsActive && g.User.IsActive)
                .Select(g => new GuideResponseDto
                {
                    GuideId = g.GuideId,
                    UserId = g.UserId,
                    FullName = g.FullName,
                    JoiningDate = g.JoiningDate,
                    Photo = g.Photo,
                    ProfessionId = g.ProfessionId,
                    ProfessionName = g.Profession != null ? g.Profession.Name : null,
                    IsActive = g.IsActive
                })
                .ToListAsync();

            return Ok(guides);
        }

        // GET: api/Guides/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GuideResponseDto>> GetGuide(int id)
        {
            var guide = await _context.Guides
                .Include(g => g.Profession)
                .Where(g => g.GuideId == id && g.IsActive && g.User.IsActive)
                .Select(g => new GuideResponseDto
                {
                    GuideId = g.GuideId,
                    UserId = g.UserId,
                    FullName = g.FullName,
                    JoiningDate = g.JoiningDate,
                    Photo = g.Photo,
                    ProfessionId = g.ProfessionId,
                    ProfessionName = g.Profession != null ? g.Profession.Name : null,
                    IsActive = g.IsActive
                })
                .FirstOrDefaultAsync();

            if (guide == null)
                return NotFound();

            return Ok(guide);
        }

        // POST: api/Guides
        [HttpPost]
        public async Task<ActionResult<GuideResponseDto>> CreateGuide(CreateGuideDto dto)
        {
            // user must exist + be active
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.IsActive);
            if (user is null)
                return BadRequest("Invalid UserId (user not found or inactive).");

            // validate profession (optional)
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.ProfessionId.Value && l.IsActive);
                if (!profExists) return BadRequest("Invalid ProfessionId.");
            }

            // guide row already exists for this user?
            var existingGuide = await _context.Guides
                .Include(g => g.Profession)
                .FirstOrDefaultAsync(g => g.UserId == dto.UserId);

            if (existingGuide != null)
            {
                if (existingGuide.IsActive)
                    return Conflict("This user is already a guide.");

                // reactivate only (POST activates; PUT updates)
                existingGuide.IsActive = true;
                await _context.SaveChangesAsync();

                var activatedResponse = new GuideResponseDto
                {
                    GuideId = existingGuide.GuideId,
                    UserId = existingGuide.UserId,
                    FullName = existingGuide.FullName,
                    JoiningDate = existingGuide.JoiningDate,
                    Photo = existingGuide.Photo,
                    ProfessionId = existingGuide.ProfessionId,
                    ProfessionName = existingGuide.Profession != null ? existingGuide.Profession.Name : null,
                    IsActive = existingGuide.IsActive
                };

                return Ok(activatedResponse); // 200 — reactivated
            }

            // create new guide
            var guide = new Guide
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                JoiningDate = dto.JoiningDate,
                Photo = dto.Photo,
                ProfessionId = dto.ProfessionId,
                IsActive = true // explicit (better for Murex)
            };

            _context.Guides.Add(guide);
            await _context.SaveChangesAsync();

            // load profession name for response
            await _context.Entry(guide).Reference(g => g.Profession).LoadAsync();

            var response = new GuideResponseDto
            {
                GuideId = guide.GuideId,
                UserId = guide.UserId,
                FullName = guide.FullName,
                JoiningDate = guide.JoiningDate,
                Photo = guide.Photo,
                ProfessionId = guide.ProfessionId,
                ProfessionName = guide.Profession != null ? guide.Profession.Name : null,
                IsActive = guide.IsActive
            };

            return CreatedAtAction(nameof(GetGuide), new { id = guide.GuideId }, response);
        }

        // PUT: api/Guides/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGuide(int id, UpdateGuideDto dto)
        {
            var guide = await _context.Guides
                .FirstOrDefaultAsync(g => g.GuideId == id && g.IsActive && g.User.IsActive);

            if (guide is null)
                return NotFound();

            // validate profession (optional)
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.ProfessionId.Value && l.IsActive);
                if (!profExists) return BadRequest("Invalid ProfessionId.");
            }

            guide.FullName = dto.FullName;
            guide.JoiningDate = dto.JoiningDate;
            guide.Photo = dto.Photo;
            guide.ProfessionId = dto.ProfessionId;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // DELETE: api/Guides/5
        // Soft delete: deactivate the Guide profile (User stays active for Member/Admin)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGuide(int id)
        {
            var guide = await _context.Guides
                .FirstOrDefaultAsync(g => g.GuideId == id && g.IsActive && g.User.IsActive);

            if (guide is null)
                return NotFound();

            guide.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
