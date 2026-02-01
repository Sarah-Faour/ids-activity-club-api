using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepo;

        public RoleService(IGenericRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            var roles = await _roleRepo.Query()
                .OrderBy(r => r.RoleName)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();

            return roles;
        }

        public async Task<RoleResponseDto?> GetByIdAsync(int id)
        {
            var role = await _roleRepo.Query()
                .Where(r => r.RoleId == id)
                .Select(r => new RoleResponseDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .FirstOrDefaultAsync();

            return role;
        }
    }
}
