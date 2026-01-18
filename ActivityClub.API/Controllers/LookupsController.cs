using ActivityClub.API.DTOs.Lookups;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LookupsController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public LookupsController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // GET: api/lookups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LookupResponseDto>>> GetLookups()
        {
            var lookups = await _context.Lookups
                .Where(l => l.IsActive)
                .OrderBy(l => l.Code)
                .ThenBy(l => l.SortOrder)
                .Select(l => new LookupResponseDto
                {
                    LookupId = l.LookupId,
                    Code = l.Code,
                    Name = l.Name,
                    SortOrder = l.SortOrder,
                    IsActive = l.IsActive
                })
                .ToListAsync();

            return Ok(lookups);
        }

        // GET: api/lookups/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LookupResponseDto>> GetLookup(int id)
        {
            var lookup = await _context.Lookups
                .Where(l => l.LookupId == id && l.IsActive)
                .Select(l => new LookupResponseDto
                {
                    LookupId = l.LookupId,
                    Code = l.Code,
                    Name = l.Name,
                    SortOrder = l.SortOrder,
                    IsActive = l.IsActive
                })
                .FirstOrDefaultAsync();

            if (lookup == null)
                return NotFound();

            return Ok(lookup);
        }

        // GET: api/lookups/code/EventStatus   (not required — better for Murex)
        [HttpGet("code/{code}")]
        public async Task<ActionResult<IEnumerable<LookupResponseDto>>> GetByCode(string code)
        {
            var lookups = await _context.Lookups
                .Where(l => l.IsActive && l.Code == code)
                .OrderBy(l => l.SortOrder)
                .ThenBy(l => l.Name)
                .Select(l => new LookupResponseDto
                {
                    LookupId = l.LookupId,
                    Code = l.Code,
                    Name = l.Name,
                    SortOrder = l.SortOrder,
                    IsActive = l.IsActive
                })
                .ToListAsync();

            return Ok(lookups);

        }
    }
}
