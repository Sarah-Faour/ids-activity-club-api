using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using ActivityClub.Contracts.Constants;
using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;

        // Reusable projection: EF can translate this to SQL
        private static readonly Expression<Func<User, UserResponseDto>> UserSelect =
            u => new UserResponseDto
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
            };

        public UserService(
            IGenericRepository<User> userRepo,
            IGenericRepository<Role> roleRepo,
            IGenericRepository<Lookup> lookupRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _lookupRepo = lookupRepo;
        }

        // ----------------------------
        // Users CRUD
        // ----------------------------

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            return await _userRepo.Query()
                .Where(u => u.IsActive)
                .OrderBy(u => u.UserId)
                .Select(UserSelect)
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            return await _userRepo.Query()
                .Where(u => u.UserId == id && u.IsActive)
                .Select(UserSelect)
                .FirstOrDefaultAsync();
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
        {
            var emailExists = await _userRepo.Query().AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already exists.");

            var genderExists = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == dto.GenderLookupId &&
                l.IsActive &&
                l.Code == LookupCodes.Gender);

            if (!genderExists)
                throw new ArgumentException("Invalid GenderLookupId.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                GenderLookupId = dto.GenderLookupId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            // Return the created user using the same projection (no Include needed)
            return await _userRepo.Query()
                .Where(u => u.UserId == user.UserId)
                .Select(UserSelect)
                .FirstAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepo.Query()
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user is null)
                return false;

            var genderExists = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == dto.GenderLookupId &&
                l.IsActive &&
                l.Code == LookupCodes.Gender);

            if (!genderExists)
                throw new ArgumentException("Invalid GenderLookupId.");

            user.Name = dto.Name;
            user.DateOfBirth = dto.DateOfBirth;
            user.GenderLookupId = dto.GenderLookupId;

            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var user = await _userRepo.Query()
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user is null)
                return false;

            user.IsActive = false;
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReactivateAsync(int id)
        {
            var user = await _userRepo.Query().FirstOrDefaultAsync(u => u.UserId == id);
            if (user is null)
                return false;

            if (user.IsActive)
                throw new InvalidOperationException("User is already active.");

            user.IsActive = true;
            await _userRepo.SaveChangesAsync();
            return true;
        }

        // ----------------------------
        // Role assignment endpoints
        // ----------------------------

        public async Task<List<RoleResponseDto>?> GetRolesAsync(int userId)
        {
            var userExists = await _userRepo.Query()
                .AnyAsync(u => u.UserId == userId && u.IsActive);

            if (!userExists)
                return null;

            return await _userRepo.Query()
                .Where(u => u.UserId == userId)
                .SelectMany(u => u.Roles)
                .OrderBy(r => r.RoleName)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();
        }

        public async Task<bool> AssignRoleAsync(int userId, int roleId)
        {
            var user = await _userRepo.Query()
                .Include(u => u.Roles) // required because we will modify the Roles collection
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

            if (user is null)
                return false;

            var role = await _roleRepo.Query().FirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role is null)
                throw new ArgumentException("Role not found.");

            if (user.Roles.Any(r => r.RoleId == roleId))
                throw new InvalidOperationException("User already has this role.");

            user.Roles.Add(role);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnassignRoleAsync(int userId, int roleId)
        {
            var user = await _userRepo.Query()
                .Include(u => u.Roles) // required because we will modify the Roles collection
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

            if (user is null)
                return false;

            var role = user.Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role is null)
                throw new ArgumentException("User does not have this role.");

            user.Roles.Remove(role);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        // ----------------------------
        // Helpers
        // ----------------------------

        private static string HashPassword(string password)
        {
            // Not production-grade (later: BCrypt / Identity). For now keep consistent with your controller.
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
