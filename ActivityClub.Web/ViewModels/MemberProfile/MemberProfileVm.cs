namespace ActivityClub.Web.ViewModels.MemberProfile
{
    public sealed class MemberProfileVm
    {
        public string FullName { get; init; } = string.Empty;
        public string? MobileNumber { get; init; }
        public string? EmergencyNumber { get; init; }

        public string JoiningDateText { get; init; } = string.Empty;

        public string? ProfessionName { get; init; }
        public string? NationalityName { get; init; }

        public string? Photo { get; init; }

        public bool IsActive { get; init; }   // ✅ view-only
    }
}