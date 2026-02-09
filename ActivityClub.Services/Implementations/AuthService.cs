using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ActivityClub.Contracts.DTOs.Auth;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ActivityClub.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IGenericRepository<User> userRepo,
            IConfiguration config,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
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

        private (string token, DateTime expiresAtUtc) GenerateJwt(User user)
        {
            var key = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"]!);

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.Name)
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

        private static string LegacySha256(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
