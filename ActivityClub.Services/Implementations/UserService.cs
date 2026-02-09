//using System.Security.Cryptography;
//using System.Text;
using ActivityClub.Contracts.Constants;
using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        // Reusable projection: EF can translate this to SQL
        /*
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
        */

        public UserService(
            IGenericRepository<User> userRepo,
            IGenericRepository<Role> roleRepo,
            IGenericRepository<Lookup> lookupRepo,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _lookupRepo = lookupRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        // ----------------------------
        // Users CRUD
        // ----------------------------

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            return await _userRepo.Query()
                .Where(u => u.IsActive)
                .OrderBy(u => u.UserId)
                .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            return await _userRepo.Query()
                .Where(u => u.UserId == id && u.IsActive)
                .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
        {
            // Best-for-Murex: normalize email check (avoid "A@x.com" vs "a@x.com")
            var emailKey = dto.Email.Trim().ToUpper();

            var emailExists = await _userRepo.Query().AnyAsync(u => u.Email.Trim().ToUpper() == emailKey);
            if (emailExists)
                throw new InvalidOperationException("Email already exists.");

            var genderExists = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == dto.GenderLookupId &&
                l.IsActive &&
                l.Code == LookupCodes.Gender);

            if (!genderExists)
                throw new ArgumentException("Invalid GenderLookupId.");

            // Map DTO -> entity (PasswordHash ignored by mapping)
            var user = _mapper.Map<User>(dto);

            
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;
            //user.PasswordHash = HashPassword(dto.Password);


            // ✅ One hashing strategy across the whole system: ASP.NET Identity hasher
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            // Return the created user using the same projection (no Include needed)
            return await _userRepo.Query()
                .Where(u => u.UserId == user.UserId)
                .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
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

            // Map updates onto tracked entity
            _mapper.Map(dto, user);

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
                .ProjectTo<RoleResponseDto>(_mapper.ConfigurationProvider)
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

        // --------------------------------------
        // Updating Email and Password endpoints
        // --------------------------------------

        public async Task<bool> UpdateEmailAsync(int id, UpdateUserEmailDto dto)
        {
            //verify that user exist and active
            var user = await _userRepo.Query()
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user is null)
                return false;

            //normalize the new email and verify that it doesn't exist
            var newEmailKey = dto.Email.Trim().ToUpper();

            var emailExists = await _userRepo.Query().AnyAsync(u =>
                u.UserId != id &&
                u.Email.Trim().ToUpper() == newEmailKey);

            if (emailExists)
                throw new InvalidOperationException("Email already exists.");

            user.Email = dto.Email.Trim();
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto)
        {
            var user = await _userRepo.Query()
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user is null)
                return false;

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);

            if (verify == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            await _userRepo.SaveChangesAsync();
            return true;
        }


        // ----------------------------
        // Helpers
        // ----------------------------

        /*
         private static string HashPassword(string password)
         {
             // Not production-grade (later: BCrypt / Identity). For now keep consistent with your controller.
             using var sha = SHA256.Create();
             var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
             return Convert.ToBase64String(bytes);
         }
        */
    }
}
