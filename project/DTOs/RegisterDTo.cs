namespace WebAppTrafficSign.DTOs
{
    public class RegisterDTo
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
