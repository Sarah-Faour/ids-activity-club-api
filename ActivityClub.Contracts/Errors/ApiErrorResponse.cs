
namespace ActivityClub.Contracts.Errors
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        // optional but very useful for debugging in dev
        public string? Details { get; set; }
    }
}
