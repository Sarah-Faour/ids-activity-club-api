using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/roles
        // Read-only roles list (roles are fixed; no CRUD)
        [AllowAnonymous] //Keeping them public avoids annoying “login required just to load static reference data(public data forever) in dropdown lists later in frontened
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        // GET: api/roles/5
        [AllowAnonymous]
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
