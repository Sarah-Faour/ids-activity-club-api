namespace ActivityClub.Contracts.DTOs.EventMembers
{
    public class EventMemberResponseDto
    {
        public int EventMemberId { get; set; }
        public int EventId { get; set; }
        public int MemberId { get; set; }
        public DateOnly JoinDate { get; set; }
        public string MemberName { get; set; } = null!;
    }
}
