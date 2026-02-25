namespace ActivityClub.Web.ViewModels.GuideProfile
{
    public class GuideProfileVm
    {
        public int GuideId { get; init; }
        public int UserId { get; init; }

        public string FullName { get; init; } = string.Empty;
        public string JoiningDateText { get; init; } = string.Empty;

        public string? Photo { get; init; }

        public int? ProfessionId { get; init; }
        public string? ProfessionName { get; init; }

        public bool IsActive { get; init; }
    }
}