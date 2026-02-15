namespace ActivityClub.Web.ViewModels.Events
{
    public sealed class EventListItemVm
    {
        public int EventId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Destination { get; init; } = string.Empty;

        public string DateFromText { get; init; } = string.Empty;
        public string DateToText { get; init; } = string.Empty;

        public decimal Cost { get; init; }
        public string? CategoryName { get; init; }
        public string? StatusName { get; init; }
    }
}


