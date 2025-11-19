namespace SharedLibrary.DTOs
{

    /// Common DTO for User information used in inter-service communication

    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}

