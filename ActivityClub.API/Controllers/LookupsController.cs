using ActivityClub.Contracts.DTOs.Lookups;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous] //Keeping them public avoids annoying “login required just to load static reference data(public data forever) in dropdown lists later in frontened
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LookupResponseDto>>> GetLookups()
        {
            var lookups = await _lookupService.GetAllAsync();
            return Ok(lookups);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LookupResponseDto>> GetLookup(int id)
        {
            var lookup = await _lookupService.GetByIdAsync(id);
            if (lookup is null) return NotFound();
            return Ok(lookup);
        }

        // (not required — better for Murex)
        [AllowAnonymous]
        [HttpGet("code/{code}")]
        public async Task<ActionResult<IEnumerable<LookupResponseDto>>> GetByCode([FromRoute] string code) //[FromRoute] tells ASP.NET that the code is coming from URL path (/code/Status) not necessary but explicit
        {
            var lookups = await _lookupService.GetByCodeAsync(code);
            return Ok(lookups);
        }
    }
}
