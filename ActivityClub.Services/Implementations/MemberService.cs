using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ActivityClub.Contracts.Constants;
using AutoMapper;
using AutoMapper.QueryableExtensions;



namespace ActivityClub.Services.Implementations
{
    public class MemberService : IMemberService
    {
        // We inject repositories, not DbContext directly (clean separation).
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;
        private readonly IMapper _mapper;


        public MemberService(
            IGenericRepository<Member> memberRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<Lookup> lookupRepo,
            IMapper mapper)
        {
            _memberRepo = memberRepo;
            _userRepo = userRepo;
            _lookupRepo = lookupRepo;
            _mapper = mapper;
        }

        public async Task<List<MemberResponseDto>> GetAllAsync()
        {
            // Best-for-Murex: ProjectTo makes EF generate SELECT only for needed DTO fields
            return await _memberRepo.Query()
                .Where(m => m.IsActive && m.User.IsActive)
                .ProjectTo<MemberResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<MemberResponseDto?> GetByIdAsync(int id)
        {
            return await _memberRepo.Query()
                .Where(m => m.MemberId == id && m.IsActive && m.User.IsActive)
                .ProjectTo<MemberResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<MemberResponseDto> CreateAsync(CreateMemberDto dto)
        {
            // From MembersController.CreateMember(dto)

            // 1) user must exist + be active
            var user = await _userRepo.Query()
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId && u.IsActive);

            if (user is null)
                throw new ArgumentException("Invalid UserId (user not found or inactive).");

            // 2) user must not already have an active member (reactivate if inactive)
            var existingMember = await _memberRepo.Query()
                .FirstOrDefaultAsync(m => m.UserId == dto.UserId);

            if (existingMember != null)
            {
                if (existingMember.IsActive)
                    throw new InvalidOperationException("This user is already a member.");

                existingMember.IsActive = true;
                await _memberRepo.SaveChangesAsync();

                // Return the same DTO shape as GET (single source of truth)
                return await _memberRepo.Query()
                    .Where(m => m.MemberId == existingMember.MemberId && m.User.IsActive)
                    .ProjectTo<MemberResponseDto>(_mapper.ConfigurationProvider)
                    .FirstAsync();
            }

            // 3) validate optional lookups ids (must be active + correct code)
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _lookupRepo.Query()
                    .AnyAsync(l => l.LookupId == dto.ProfessionId.Value && l.Code == LookupCodes.Profession && l.IsActive);

                if (!profExists) throw new ArgumentException("Invalid ProfessionId (must be an active profession lookup).");
            }

            if (dto.NationalityId.HasValue)
            {
                var natExists = await _lookupRepo.Query()
                    .AnyAsync(l => l.LookupId == dto.NationalityId.Value && l.Code == LookupCodes.Nationality && l.IsActive);

                if (!natExists) throw new ArgumentException("Invalid NationalityId (must be an active nationality lookup)");
            }

            // 4) map DTO -> entity (AutoMapper)
            var member = _mapper.Map<Member>(dto);
            member.IsActive = true; // enforce your business rule explicitly

            await _memberRepo.AddAsync(member);
            await _memberRepo.SaveChangesAsync();

            // 5) best-for-Murex: re-query + ProjectTo (one consistent DTO shape)
            return await _memberRepo.Query()
                .Where(m => m.MemberId == member.MemberId)
                .ProjectTo<MemberResponseDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            // how to use the methods to load
            // await _memberRepo.LoadReferenceAsync(member, m => m.Profession);
            //await _memberRepo.LoadReferenceAsync(member, m => m.Nationality);

            // return new MemberResponseDto
            //{
            //  MemberId = member.MemberId,
            //UserId = member.UserId,
            //FullName = member.FullName,
            //MobileNumber = member.MobileNumber,
            //EmergencyNumber = member.EmergencyNumber,
            //JoiningDate = member.JoiningDate,
            //Photo = member.Photo,
            //ProfessionId = member.ProfessionId,
            //ProfessionName = member.Profession != null ? member.Profession.Name : null,
            //NationalityId = member.NationalityId,
            //NationalityName = member.Nationality != null ? member.Nationality.Name : null,
            //IsActive = member.IsActive
            //};
        }

        public async Task<bool> UpdateAsync(int id, UpdateMemberDto dto)
        {
            // From MembersController.UpdateMember(id, dto)
            var member = await _memberRepo.Query()
                .FirstOrDefaultAsync(m => m.MemberId == id && m.IsActive && m.User.IsActive);

            if (member is null)
                return false;

            // validate optional lookup ids
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _lookupRepo.Query()
                   .AnyAsync(l => l.LookupId == dto.ProfessionId.Value && l.Code == LookupCodes.Profession && l.IsActive);

                if (!profExists) throw new ArgumentException("Invalid ProfessionId (must be an active profession lookup).");
            }

            if (dto.NationalityId.HasValue)
            {
                var natExists = await _lookupRepo.Query()
                    .AnyAsync(l => l.LookupId == dto.NationalityId.Value && l.Code == LookupCodes.Nationality && l.IsActive);

                if (!natExists) throw new ArgumentException("Invalid NationalityId (must be an active nationality lookup)");
            }

            // Map updates onto tracked entity (Memory -> DB once SaveChanges happens)
            _mapper.Map(dto, member);

            await _memberRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            // From MembersController.DeleteMember(id)
            var member = await _memberRepo.Query()
                .FirstOrDefaultAsync(m => m.MemberId == id && m.IsActive && m.User.IsActive);

            if (member is null)
                return false;

            member.IsActive = false;
            await _memberRepo.SaveChangesAsync();
            return true;
        }

        // ----------------------------
        // Self-Service
        // ----------------------------
        public async Task<MemberResponseDto?> GetMyProfileAsync(int userId)
        {
            return await _memberRepo.Query()
                .Where(m => m.UserId == userId && m.IsActive && m.User.IsActive)
                .ProjectTo<MemberResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateMyProfileAsync(int userId, UpdateMemberDto dto)
        {
            var member = await _memberRepo.Query()
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive && m.User.IsActive);

            if (member is null)
                return false;

            // validate optional lookup ids
            if (dto.ProfessionId.HasValue)
            {
                var profExists = await _lookupRepo.Query()
                   .AnyAsync(l => l.LookupId == dto.ProfessionId.Value
                                  && l.Code == LookupCodes.Profession
                                  && l.IsActive);

                if (!profExists)
                    throw new ArgumentException("Invalid ProfessionId (must be an active profession lookup).");
            }

            if (dto.NationalityId.HasValue)
            {
                var natExists = await _lookupRepo.Query()
                    .AnyAsync(l => l.LookupId == dto.NationalityId.Value
                                   && l.Code == LookupCodes.Nationality
                                   && l.IsActive);

                if (!natExists)
                    throw new ArgumentException("Invalid NationalityId (must be an active nationality lookup).");
            }

            _mapper.Map(dto, member);

            await _memberRepo.SaveChangesAsync();
            return true;
        }
    }
}

