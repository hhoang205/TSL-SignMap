namespace SharedLibrary.DTOs
{

    /// Standard error response DTO for API responses

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
        public int? StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

