namespace ActivityClub.Web.ViewModels.Events
{
    public sealed class EventDetailsVm
    {
        public int EventId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string Destination { get; init; } = string.Empty;

        // NEW: actual dates (for logic)
        public DateOnly DateFrom { get; init; }
        public DateOnly DateTo { get; init; }

        // existing display fields
        public string DateFromText { get; init; } = string.Empty;
        public string DateToText { get; init; } = string.Empty;

        public decimal Cost { get; init; }
        public string? CategoryName { get; init; }
        public string? StatusName { get; init; }
    }
}