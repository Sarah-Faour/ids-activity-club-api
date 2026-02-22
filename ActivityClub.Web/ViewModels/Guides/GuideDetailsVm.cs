namespace ActivityClub.Web.ViewModels.Guides
{
    public sealed class GuideDetailsVm
    {
        public int GuideId { get; init; }
        public int UserId { get; init; }

        public string FullName { get; init; } = string.Empty;

        public string JoiningDateText { get; init; } = string.Empty;

        public int? ProfessionId { get; init; }
        public string? ProfessionName { get; init; }

        public string? Photo { get; init; }

        public bool IsActive { get; init; }
    }
}