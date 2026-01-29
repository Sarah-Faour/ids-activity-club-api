using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuidesController : ControllerBase
    {
        private readonly IGuideService _guideService;

        public GuidesController(IGuideService guideService)
        {
            _guideService = guideService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuideResponseDto>>> GetGuides()
        {
            var guides = await _guideService.GetAllAsync();
            return Ok(guides);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GuideResponseDto>> GetGuide(int id)
        {
            var guide = await _guideService.GetByIdAsync(id);
            if (guide is null) return NotFound();
            return Ok(guide);
        }

        [HttpPost]
        public async Task<ActionResult<GuideResponseDto>> CreateGuide(CreateGuideDto dto)
        {
            var created = await _guideService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetGuide), new { id = created.GuideId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGuide(int id, UpdateGuideDto dto)
        {
            var updated = await _guideService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGuide(int id)
        {
            var deleted = await _guideService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
