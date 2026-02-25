using ActivityClub.Contracts.DTOs.Members;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IMemberProfileApiClient
    {
        Task<MemberResponseDto> GetMeAsync(CancellationToken ct = default);
        Task UpdateMeAsync(UpdateMemberDto dto, CancellationToken ct = default);
    }
}