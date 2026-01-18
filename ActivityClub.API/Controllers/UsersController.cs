using System.Security.Cryptography;
using System.Text;
using ActivityClub.API.DTOs.Roles;
using ActivityClub.API.DTOs.Users;
using ActivityClub.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class UsersController : ControllerBase
    {
        private readonly ActivityClubDbContext _context;

        public UsersController(ActivityClubDbContext context)
        {
            _context = context;
        }

        // ----------------------------
        // Users CRUD
        // ----------------------------

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.UserId)
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    DateOfBirth = u.DateOfBirth,
                    GenderLookupId = u.GenderLookupId,
                    GenderName = u.GenderLookup.Name,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    Roles = u.Roles.Select(r => new RoleResponseDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName
                    }).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id && u.IsActive)
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    DateOfBirth = u.DateOfBirth,
                    GenderLookupId = u.GenderLookupId,
                    GenderName = u.GenderLookup.Name,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    Roles = u.Roles.Select(r => new RoleResponseDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto)
        {
            // email must be unique
            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return Conflict("Email already exists.");

            // gender must exist + active (Lookup)
            var genderExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.GenderLookupId && l.IsActive);
            if (!genderExists)
                return BadRequest("Invalid GenderLookupId.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                GenderLookupId = dto.GenderLookupId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Load Gender for response name
            await _context.Entry(user).Reference(x => x.GenderLookup).LoadAsync();

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                GenderLookupId = user.GenderLookupId,
                GenderName = user.GenderLookup.Name,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = new List<RoleResponseDto>() // empty initially
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, response);
        }

        // PUT: api/users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);
            if (user is null)
                return NotFound();

            // validate gender lookup
            var genderExists = await _context.Lookups.AnyAsync(l => l.LookupId == dto.GenderLookupId && l.IsActive);
            if (!genderExists)
                return BadRequest("Invalid GenderLookupId.");

            user.Name = dto.Name;
            user.DateOfBirth = dto.DateOfBirth;
            user.GenderLookupId = dto.GenderLookupId;

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // DELETE: api/users/5  (soft delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);
            if (user is null)
                return NotFound();

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // POST: api/users/5/reactivate
        [HttpPost("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user is null)
                return NotFound();

            if (user.IsActive)
                return Conflict("User is already active.");

            user.IsActive = true;
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // ----------------------------
        // Role assignment endpoints
        // ----------------------------

        // GET: api/users/5/roles
        [HttpGet("{id:int}/roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetUserRoles(int id)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == id && u.IsActive);
            if (!userExists)
                return NotFound("User not found or inactive.");

            var roles = await _context.Users
                .Where(u => u.UserId == id)
                .SelectMany(u => u.Roles)
                .OrderBy(r => r.RoleName)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();

            return Ok(roles);
        }

        // POST: api/users/5/roles/2
        [HttpPost("{id:int}/roles/{roleId:int}")]
        public async Task<IActionResult> AssignRole(int id, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user is null)
                return NotFound("User not found or inactive.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role is null)
                return NotFound("Role not found.");

            var alreadyHasRole = user.Roles.Any(r => r.RoleId == roleId);
            if (alreadyHasRole)
                return Conflict("User already has this role.");

            user.Roles.Add(role);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // DELETE: api/users/5/roles/2
        [HttpDelete("{id:int}/roles/{roleId:int}")]
        public async Task<IActionResult> UnassignRole(int id, int roleId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user is null)
                return NotFound("User not found or inactive.");

            var role = user.Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role is null)
                return NotFound("User does not have this role.");

            user.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // ----------------------------
        // Simple password hashing (temporary)
        // ----------------------------

        private static string HashPassword(string password)
        {
            // (not required — better for Murex later) Replace with ASP.NET Identity / BCrypt
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
