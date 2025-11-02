using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Mapper
{
    public static class NotificationMapper
    {
        public static NotificationDto toDto(this Notification notification)
        {
            return new NotificationDto()
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}

