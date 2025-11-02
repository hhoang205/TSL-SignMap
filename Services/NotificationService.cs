using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Mapper;

namespace WebAppTrafficSign.Services
{
    /// Service quản lý Notifications theo requirement
    /// - Users receive real-time notifications about approved changes and submission outcomes
    /// - Notifications cho contribution status (approved, rejected, pending)
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Lấy notification theo ID
        public async Task<NotificationDto> GetByIdAsync(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
                throw new InvalidOperationException("Notification not found");

            return notification.toDto();
        }

        /// Lấy tất cả notifications của user (theo thứ tự mới nhất)
        public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => n.toDto());
        }

        /// Lấy notifications chưa đọc của user
        public async Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => n.toDto());
        }

        /// Đếm số notifications chưa đọc của user
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        /// Tạo notification mới
        public async Task<NotificationDto> CreateAsync(NotificationCreateRequest request)
        {
            // Validate user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
                throw new InvalidOperationException("User not found");

            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(notification)
                .Reference(n => n.User).LoadAsync();

            return notification.toDto();
        }

        /// Đánh dấu notification là đã đọc
        public async Task<NotificationDto> MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
                throw new InvalidOperationException("Notification not found");

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return notification.toDto();
        }

        /// Đánh dấu tất cả notifications của user là đã đọc
        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// Xóa notification
        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        /// Filter notifications theo các criteria
        public async Task<IEnumerable<NotificationDto>> FilterAsync(NotificationFilterRequest request)
        {
            IQueryable<Notification> query = _context.Notifications
                .Include(n => n.User);

            // Filter by userId (required)
            query = query.Where(n => n.UserId == request.UserId);

            // Filter by IsRead
            if (request.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == request.IsRead.Value);
            }

            // Filter by date range
            if (request.StartDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt <= request.EndDate.Value);
            }

            // Filter by title/message search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                query = query.Where(n => 
                    n.Title.ToLower().Contains(searchLower) || 
                    n.Message.ToLower().Contains(searchLower));
            }

            // Order by CreatedAt descending
            query = query.OrderByDescending(n => n.CreatedAt);

            // Pagination
            if (request.PageNumber > 0 && request.PageSize > 0)
            {
                query = query.Skip((request.PageNumber - 1) * request.PageSize)
                             .Take(request.PageSize);
            }

            var notifications = await query.ToListAsync();
            return notifications.Select(n => n.toDto());
        }
    }
}

