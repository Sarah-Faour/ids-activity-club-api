using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ActivityClub.Contracts.Constants;


namespace ActivityClub.Services.Implementations
{
    public class MemberService : IMemberService
    {
        // We inject repositories, not DbContext directly (clean separation).
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Lookup> _lookupRepo;

        public MemberService(
            IGenericRepository<Member> memberRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<Lookup> lookupRepo)
        {
            _memberRepo = memberRepo;
            _userRepo = userRepo;
            _lookupRepo = lookupRepo;
        }

        public async Task<List<MemberResponseDto>> GetAllAsync()
        {
            // From MembersController.GetMembers(): same filter, includes, and projection to DTO.
            var members = await _memberRepo.Query()
                .Include(m => m.Profession)
                .Include(m => m.Nationality)
                .Where(m => m.IsActive && m.User.IsActive)
                .Select(m => new MemberResponseDto
                {
                    MemberId = m.MemberId,
                    UserId = m.UserId,
                    FullName = m.FullName,
                    MobileNumber = m.MobileNumber,
                    EmergencyNumber = m.EmergencyNumber,
                    JoiningDate = m.JoiningDate,
                    Photo = m.Photo,
                    ProfessionId = m.ProfessionId,
                    ProfessionName = m.Profession != null ? m.Profession.Name : null,
                    NationalityId = m.NationalityId,
                    NationalityName = m.Nationality != null ? m.Nationality.Name : null,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return members;
        }

        public async Task<MemberResponseDto?> GetByIdAsync(int id)
        {
            // From MembersController.GetMember(id)
            var member = await _memberRepo.Query()
                .Include(m => m.Profession)
                .Include(m => m.Nationality)
                .Where(m => m.MemberId == id && m.IsActive && m.User.IsActive)
                .Select(m => new MemberResponseDto
                {
                    MemberId = m.MemberId,
                    UserId = m.UserId,
                    FullName = m.FullName,
                    MobileNumber = m.MobileNumber,
                    EmergencyNumber = m.EmergencyNumber,
                    JoiningDate = m.JoiningDate,
                    Photo = m.Photo,
                    ProfessionId = m.ProfessionId,
                    ProfessionName = m.Profession != null ? m.Profession.Name : null,
                    NationalityId = m.NationalityId,
                    NationalityName = m.Nationality != null ? m.Nationality.Name : null,
                    IsActive = m.IsActive
                })
                .FirstOrDefaultAsync();

            return member; // null means "not found"
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
                .Include(m => m.Profession)
                .Include(m => m.Nationality)
                .FirstOrDefaultAsync(m => m.UserId == dto.UserId);

            if (existingMember != null)
            {
                if (existingMember.IsActive)
                    throw new InvalidOperationException("This user is already a member.");

                existingMember.IsActive = true;
                await _memberRepo.SaveChangesAsync();

                return new MemberResponseDto
                {
                    MemberId = existingMember.MemberId,
                    UserId = existingMember.UserId,
                    FullName = existingMember.FullName,
                    MobileNumber = existingMember.MobileNumber,
                    EmergencyNumber = existingMember.EmergencyNumber,
                    JoiningDate = existingMember.JoiningDate,
                    Photo = existingMember.Photo,
                    ProfessionId = existingMember.ProfessionId,
                    ProfessionName = existingMember.Profession != null ? existingMember.Profession.Name : null,
                    NationalityId = existingMember.NationalityId,
                    NationalityName = existingMember.Nationality != null ? existingMember.Nationality.Name : null,
                    IsActive = existingMember.IsActive
                };
            }

            // 3) validate optional lookup ids
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

            // 4) create entity
            var member = new Member
            {
                UserId = dto.UserId,
                FullName = dto.FullName,
                MobileNumber = dto.MobileNumber,
                EmergencyNumber = dto.EmergencyNumber,
                JoiningDate = dto.JoiningDate,
                Photo = dto.Photo,
                ProfessionId = dto.ProfessionId,
                NationalityId = dto.NationalityId,
                IsActive = true
            };

            await _memberRepo.AddAsync(member);
            await _memberRepo.SaveChangesAsync();

            await _memberRepo.LoadReferenceAsync(member, m => m.Profession);
            await _memberRepo.LoadReferenceAsync(member, m => m.Nationality);

            return new MemberResponseDto
            {
                MemberId = member.MemberId,
                UserId = member.UserId,
                FullName = member.FullName,
                MobileNumber = member.MobileNumber,
                EmergencyNumber = member.EmergencyNumber,
                JoiningDate = member.JoiningDate,
                Photo = member.Photo,
                ProfessionId = member.ProfessionId,
                ProfessionName = member.Profession != null ? member.Profession.Name : null,
                NationalityId = member.NationalityId,
                NationalityName = member.Nationality != null ? member.Nationality.Name : null,
                IsActive = member.IsActive
            };
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

            member.FullName = dto.FullName;
            member.MobileNumber = dto.MobileNumber;
            member.EmergencyNumber = dto.EmergencyNumber;
            member.JoiningDate = dto.JoiningDate;
            member.Photo = dto.Photo;
            member.ProfessionId = dto.ProfessionId;
            member.NationalityId = dto.NationalityId;

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
    }
}

