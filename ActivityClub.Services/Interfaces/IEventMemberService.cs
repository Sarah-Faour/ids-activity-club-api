using ActivityClub.Contracts.DTOs.EventMembers;

namespace ActivityClub.Services.Interfaces
{
    public interface IEventMemberService
    {
        Task<List<EventMemberResponseDto>> GetMembersForEventAsync(int eventId);
        Task AssignMemberToEventAsync(int eventId, int memberId);
        Task<bool> UnassignMemberFromEventAsync(int eventId, int memberId);
        Task JoinEventAsync(int eventId, int userId); // NEW: member self-join (better for Murex + required by Full Project public flow)
    }
}
