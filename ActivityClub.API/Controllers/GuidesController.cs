using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT required by default
    public class GuidesController : ControllerBase
    {
        private readonly IGuideService _guideService;

        public GuidesController(IGuideService guideService)
        {
            _guideService = guideService;
        }

        // GET: api/guides (authenticated)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuideResponseDto>>> GetGuides()
        {
            var guides = await _guideService.GetAllAsync();
            return Ok(guides);
        }

        // GET: api/guides/5 (authenticated)
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GuideResponseDto>> GetGuide(int id)
        {
            var guide = await _guideService.GetByIdAsync(id);
            if (guide is null) return NotFound();
            return Ok(guide);
        }

        // POST: api/guides (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GuideResponseDto>> CreateGuide([FromBody] CreateGuideDto dto)
        {
            var created = await _guideService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetGuide), new { id = created.GuideId }, created);
        }

        // PUT: api/guides/5 (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGuide(int id, [FromBody] UpdateGuideDto dto)
        {
            var updated = await _guideService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/guides/5 (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGuide(int id)
        {
            var deleted = await _guideService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
