using ActivityClub.Contracts.DTOs.Members;

namespace ActivityClub.Services.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberResponseDto>> GetAllAsync();
        Task<MemberResponseDto?> GetByIdAsync(int id);
        Task<MemberResponseDto> CreateAsync(CreateMemberDto dto);
        Task<bool> UpdateAsync(int id, UpdateMemberDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}
