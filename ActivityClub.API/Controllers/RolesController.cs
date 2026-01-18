using ActivityClub.API.DTOs.Roles;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public RolesController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // GET: api/roles
        // Read-only roles list (roles are fixed; no CRUD)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .OrderBy(r => r.RoleName)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/roles/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleResponseDto>> GetRole(int id)
        {
            var role = await _context.Roles
                .Where(r => r.RoleId == id)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .FirstOrDefaultAsync();

            if (role is null)
                return NotFound();

            return Ok(role);
        }
    }
}
