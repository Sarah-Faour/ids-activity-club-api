namespace ActivityClub.Web.ViewModels.Profile
{
    public sealed class ProfileVm
    {
        // Not displayed by default, but useful internally (links/debug)
        public int UserId { get; init; }

        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;

        // Display-friendly strings (because Razor view shouldn’t format logic everywhere)
        public string DateOfBirthText { get; init; } = string.Empty;
        public string GenderName { get; init; } = string.Empty;

        public string CreatedAtText { get; init; } = string.Empty;
        public bool IsActive { get; init; }

        public List<string> Roles { get; init; } = new();
    }
}