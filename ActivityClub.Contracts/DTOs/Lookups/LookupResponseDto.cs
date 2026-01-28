namespace ActivityClub.Contracts.DTOs.Lookups
{
    public class LookupResponseDto
    {
        public int LookupId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
