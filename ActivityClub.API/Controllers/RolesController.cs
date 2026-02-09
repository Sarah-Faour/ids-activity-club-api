using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // all endpoints require authentication
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/roles
        // Read-only roles list (roles are fixed; no CRUD)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        // GET: api/roles/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleResponseDto>> GetRole(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role is null)
                return NotFound();

            return Ok(role);
        }
    }
}
