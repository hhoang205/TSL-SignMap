namespace WebAppTrafficSign.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
