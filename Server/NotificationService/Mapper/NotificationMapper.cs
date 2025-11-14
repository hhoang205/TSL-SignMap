using NotificationService.Models;
using NotificationService.DTOs;

namespace NotificationService.Mapper
{
    public static class NotificationMapper
    {
        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
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

