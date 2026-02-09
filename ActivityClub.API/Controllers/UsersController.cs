using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // all endpoints require a valid JWT by default
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ----------------------------
        // Users CRUD
        // ----------------------------

        // GET: api/users  (authenticated)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/users/5  (authenticated)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null) return NotFound();
            return Ok(user);
        }

        // POST: api/users  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto dto) //[FromBody] tells ASP.NET this parameter comes from the HTTP request body (JSON), where [ApiController] makes it optional, but explicit is clean.
        {
            var created = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
        }

        // PUT: api/users/5  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/users/5  (ADMIN only)
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // POST: api/users/5/reactivate  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            var ok = await _userService.ReactivateAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ----------------------------
        // Role Assignement endpoints
        // ----------------------------

        // GET: api/users/5/roles  (authenticated)
        [HttpGet("{id:int}/roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetUserRoles(int id)
        {
            var roles = await _userService.GetRolesAsync(id);
            if (roles is null) return NotFound();
            return Ok(roles);
        }

        // POST: api/users/5/roles/2  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/roles/{roleId:int}")]
        public async Task<IActionResult> AssignRole(int id, int roleId)
        {
            var ok = await _userService.AssignRoleAsync(id, roleId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE: api/users/5/roles/2  (ADMIN only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}/roles/{roleId:int}")]
        public async Task<IActionResult> UnassignRole(int id, int roleId)
        {
            var ok = await _userService.UnassignRoleAsync(id, roleId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // --------------------------------------
        // Updating Email and Password endpoints
        // --------------------------------------

        // PUT: api/users/5/email  (ADMIN only — for now)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/email")]
        public async Task<IActionResult> UpdateEmail(int id, [FromBody] UpdateUserEmailDto dto)
        {
            var ok = await _userService.UpdateEmailAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        // PUT: api/users/5/password  (ADMIN only — for now)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
        {
            var ok = await _userService.ChangePasswordAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

    }
}
