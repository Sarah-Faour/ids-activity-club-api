using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

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
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuideResponseDto>>> GetGuides()
        {
            var guides = await _guideService.GetAllAsync();
            return Ok(guides);
        }

        // GET: api/guides/5 (authenticated)
        [AllowAnonymous]
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

        // POST: api/guides/{id}/reactivate  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateGuide(int id)
        {
            var ok = await _guideService.ReactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // GET: api/guides/admin (ADMIN only) -> active + inactive
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<GuideResponseDto>>> GetAllForAdmin()
        {
            var guides = await _guideService.GetAllForAdminAsync();
            return Ok(guides);
        }

        // ----------------------------
        // /me endpoints (self-service)
        // ----------------------------

        [Authorize(Roles = "Guide")]
        [HttpGet("me")]
        public async Task<ActionResult<GuideResponseDto>> GetMyGuideProfile()
        {
            var myId = GetCurrentUserId();
            if (myId is null) return Unauthorized();

            var guide = await _guideService.GetMyProfileAsync(myId.Value);
            if (guide is null) return NotFound();

            return Ok(guide);
        }

        [Authorize(Roles = "Guide")]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyGuideProfile([FromBody] UpdateGuideDto dto)
        {
            var myId = GetCurrentUserId();
            if (myId is null) return Unauthorized();

            var updated = await _guideService.UpdateMyProfileAsync(myId.Value, dto);
            if (!updated) return NotFound();

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
