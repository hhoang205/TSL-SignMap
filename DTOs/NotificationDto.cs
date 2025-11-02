namespace WebAppTrafficSign.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationCreateRequest
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class NotificationFilterRequest
    {
        public int UserId { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class NotificationUnreadCountResponse
    {
        public int UserId { get; set; }
        public int UnreadCount { get; set; }
    }
}
