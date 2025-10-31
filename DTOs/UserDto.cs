namespace WebAppTrafficSign.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public float Reputation { get; set; }
        public int RoleId { get; set; }  // 0: user, 1: staff, 2: admin
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class UserRegistrationRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class AuthResponse
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
    public class UserUpdateRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }

    /// Response khi lấy số dư coin trong ví
    public class WalletBalanceResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
    }

    /// Request nạp coin vào ví
    public class CoinTopUpRequest
    {
        public decimal AmountInDollars { get; set; }
        public string PaymentMethod { get; set; } = "Credit Card";
    }

    /// Response sau khi nạp coin thành công
    public class TopUpCoinsResponse
    {
        public int UserId { get; set; }
        public decimal AmountInDollars { get; set; }
        public decimal CoinsAdded { get; set; }
        public decimal NewBalance { get; set; }
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    /// Response thông tin đầy đủ profile user
    public class UserProfileResponse
    {
        public UserDto User { get; set; }
        public decimal CoinBalance { get; set; }
        public int TotalContributions { get; set; }
        public int TotalVotes { get; set; }
    }
}
