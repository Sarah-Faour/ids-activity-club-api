using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null) return NotFound();
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto)
        {
            var created = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // POST: api/users/5/reactivate
        [HttpPost("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            var ok = await _userService.ReactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // GET: api/users/5/roles
        [HttpGet("{id:int}/roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetUserRoles(int id)
        {
            var roles = await _userService.GetRolesAsync(id);
            if (roles is null) return NotFound();
            return Ok(roles);
        }

        // POST: api/users/5/roles/2
        [HttpPost("{id:int}/roles/{roleId:int}")]
        public async Task<IActionResult> AssignRole(int id, int roleId)
        {
            var ok = await _userService.AssignRoleAsync(id, roleId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE: api/users/5/roles/2
        [HttpDelete("{id:int}/roles/{roleId:int}")]
        public async Task<IActionResult> UnassignRole(int id, int roleId)
        {
            var ok = await _userService.UnassignRoleAsync(id, roleId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
