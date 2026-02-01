using ActivityClub.Contracts.DTOs.EventMembers;

namespace ActivityClub.Services.Interfaces
{
    public interface IEventMemberService
    {
        Task<List<EventMemberResponseDto>> GetMembersForEventAsync(int eventId);
        Task AssignMemberToEventAsync(int eventId, int memberId);
        Task<bool> UnassignMemberFromEventAsync(int eventId, int memberId);
    }
}
