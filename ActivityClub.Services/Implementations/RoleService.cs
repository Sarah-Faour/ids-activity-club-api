using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IMapper _mapper;

        public RoleService(IGenericRepository<Role> roleRepo, IMapper mapper)
        {
            _roleRepo = roleRepo;
            _mapper = mapper;
        }

        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            var roles = await _roleRepo.Query()
                .OrderBy(r => r.RoleName)
                .ProjectTo<RoleResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return roles;
        }

        public async Task<RoleResponseDto?> GetByIdAsync(int id)
        {
            return await _roleRepo.Query()
                .Where(r => r.RoleId == id)
                .ProjectTo<RoleResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

        }
    }
}
