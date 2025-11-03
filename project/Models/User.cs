

namespace WebAppTrafficSign.Models
{
    public class User 
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Lastname { get; set; }
        public string? Firstname { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public float Reputation { get; set; }
        public CoinWallet Wallet { get; set; }
        public List<Notification> Notification { get; set; } = new List<Notification>();
        public List<Feedback> Feedbacks {  get; set; } =new List<Feedback>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }


}
