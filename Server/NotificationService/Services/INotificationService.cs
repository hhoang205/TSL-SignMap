using NotificationService.DTOs;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> GetByIdAsync(int id);
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<NotificationDto> CreateAsync(NotificationCreateRequest request);
        Task<NotificationDto> MarkAsReadAsync(int id);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<NotificationDto>> FilterAsync(NotificationFilterRequest request);
    }
}

