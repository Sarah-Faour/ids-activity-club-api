using ActivityClub.Contracts.Constants;
using ActivityClub.Contracts.DTOs.Auth;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using System.Security.Cryptography;
using System.Text;

namespace ActivityClub.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly ActivityClubDbContext _db; // for transaction
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IGenericRepository<User> userRepo,
            IGenericRepository<Role> roleRepo,
            IGenericRepository<Lookup> lookupRepo,
            IGenericRepository<Member> memberRepo,
            ActivityClubDbContext db,
            IConfiguration config,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _lookupRepo = lookupRepo;
            _memberRepo = memberRepo;
            _db = db;
            _config = config;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var emailKey = dto.Email.Trim().ToUpper();

            var user = await _userRepo.Query()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u =>
                    u.IsActive &&
                    u.Email.Trim().ToUpper() == emailKey);

            if (user is null)
                return null;

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            /*
            if (verify == PasswordVerificationResult.Failed)
            {
                // Optional legacy support (if old users were saved with SHA256)
                var legacyHash = LegacySha256(dto.Password);
                if (user.PasswordHash != legacyHash)
                    return null;

                // Upgrade legacy hash to Identity hash after a successful legacy login
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                _userRepo.Update(user);
                await _userRepo.SaveChangesAsync();
            }
            */

            if (verify == PasswordVerificationResult.Failed)
                return null;

            else if (verify == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Upgrade hash if framework says the stored hash is using older parameters
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                _userRepo.Update(user);
                await _userRepo.SaveChangesAsync();
            }

            var (token, expiresAtUtc) = GenerateJwt(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Roles = user.Roles.Select(r => r.RoleName).ToList()
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            // Transaction = all-or-nothing because we are holding multiple aggregates at the same time
            await using var tx = await _db.Database.BeginTransactionAsync();

            // 1) Email unique (case-insensitive)
            var emailKey = dto.Email.Trim().ToUpper();
            var emailExists = await _userRepo.Query()
                .AnyAsync(u => u.Email.Trim().ToUpper() == emailKey);

            if (emailExists)
                throw new InvalidOperationException("Email already exists.");

            // 2) Validate GenderLookupId (exists + active + code=Gender)
            var genderExists = await _lookupRepo.Query().AnyAsync(l =>
                l.LookupId == dto.GenderLookupId &&
                l.IsActive &&
                l.Code == LookupCodes.Gender);

            if (!genderExists)
                throw new ArgumentException("Invalid GenderLookupId.");

            // 3) Load default role = Member
            // (Option A you chose: Guide is ALSO Member; Admin can later add Guide/Admin roles)
            var memberRole = await _roleRepo.Query()
                .FirstOrDefaultAsync(r => r.RoleName == "Member");

            if (memberRole is null)
                throw new InvalidOperationException("Default role 'Member' not found.");

            // 4) Create user
            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                DateOfBirth = dto.DateOfBirth,
                GenderLookupId = dto.GenderLookupId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            // IMPORTANT: attach role
            user.Roles.Add(memberRole);

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync(); // ensures user.UserId exists

            // 5) Create Member profile (Option A)
            var member = new Member
            {
                UserId = user.UserId,
                FullName = user.Name, // default = user's name
                JoiningDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsActive = true
            };

            await _memberRepo.AddAsync(member);
            await _memberRepo.SaveChangesAsync();

            await tx.CommitAsync();

            // 6) Generate token with roles
            // If EF didn’t load roles (depends on tracking), ensure role exists in memory already.
            var (token, expiresAtUtc) = GenerateJwt(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                UserId = user.UserId,
                Email = user.Email,
                Name = user.Name,
                Roles = user.Roles.Select(r => r.RoleName).ToList()
            };
        }


        private (string token, DateTime expiresAtUtc) GenerateJwt(User user)
        {
            var key = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"]!);

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), //standard claim
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.Name) //custom cliam
            };

            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (token, expiresAtUtc);
        }

        /*
        private static string LegacySha256(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        */
    }
}
