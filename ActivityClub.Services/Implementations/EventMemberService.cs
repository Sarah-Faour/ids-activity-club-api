using ActivityClub.Contracts.DTOs.EventMembers;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Services.Implementations
{
    public class EventMemberService : IEventMemberService
    {
        private readonly IGenericRepository<Event> _eventRepo;
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<EventMember> _eventMemberRepo;

        public EventMemberService(
            IGenericRepository<Event> eventRepo,
            IGenericRepository<Member> memberRepo,
            IGenericRepository<EventMember> eventMemberRepo)
        {
            _eventRepo = eventRepo;
            _memberRepo = memberRepo;
            _eventMemberRepo = eventMemberRepo;
        }

        public async Task<List<EventMemberResponseDto>> GetMembersForEventAsync(int eventId)
        {
            // Event must exist and be active
            var eventExists = await _eventRepo.Query()
                .AnyAsync(e => e.EventId == eventId && e.IsActive);

            if (!eventExists)
                throw new ArgumentException("Event not found or inactive.");

            // Projection (best) — no Include needed
            var members = await _eventMemberRepo.Query()
                .Where(em =>
                    em.EventId == eventId &&
                    em.Member.IsActive &&
                    em.Member.User.IsActive)
                .Select(em => new EventMemberResponseDto
                {
                    EventMemberId = em.EventMemberId,
                    EventId = em.EventId,
                    MemberId = em.MemberId,
                    JoinDate = em.JoinDate,
                    MemberName = em.Member.FullName
                })
                .ToListAsync();

            return members;
        }

        public async Task AssignMemberToEventAsync(int eventId, int memberId)
        {
            // Validate event
            var evExists = await _eventRepo.Query()
                .AnyAsync(e => e.EventId == eventId && e.IsActive);

            if (!evExists)
                throw new ArgumentException("Event not found or inactive.");

            // Validate member + user active
            var memberExists = await _memberRepo.Query()
                .AnyAsync(m => m.MemberId == memberId && m.IsActive && m.User.IsActive);

            if (!memberExists)
                throw new ArgumentException("Member not found or inactive.");

            // Prevent duplicates
            var alreadyAssigned = await _eventMemberRepo.Query()
                .AnyAsync(em => em.EventId == eventId && em.MemberId == memberId);

            if (alreadyAssigned)
                throw new InvalidOperationException("Member is already assigned to this event.");

            var link = new EventMember
            {
                EventId = eventId,
                MemberId = memberId,
                JoinDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _eventMemberRepo.AddAsync(link);
            await _eventMemberRepo.SaveChangesAsync();
        }

        public async Task<bool> UnassignMemberFromEventAsync(int eventId, int memberId)
        {
            var link = await _eventMemberRepo.Query()
                .FirstOrDefaultAsync(em => em.EventId == eventId && em.MemberId == memberId);

            if (link is null)
                return false;

            _eventMemberRepo.Remove(link);
            await _eventMemberRepo.SaveChangesAsync();
            return true;
        }
    }
}
