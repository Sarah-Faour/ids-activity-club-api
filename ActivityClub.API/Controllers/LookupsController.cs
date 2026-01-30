using ActivityClub.Contracts.DTOs.Lookups;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupsController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LookupResponseDto>>> GetLookups()
        {
            var lookups = await _lookupService.GetAllAsync();
            return Ok(lookups);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LookupResponseDto>> GetLookup(int id)
        {
            var lookup = await _lookupService.GetByIdAsync(id);
            if (lookup is null) return NotFound();
            return Ok(lookup);
        }

        // (not required — better for Murex)
        [HttpGet("code/{code}")]
        public async Task<ActionResult<IEnumerable<LookupResponseDto>>> GetByCode(string code)
        {
            var lookups = await _lookupService.GetByCodeAsync(code);
            return Ok(lookups);
        }
    }
}
