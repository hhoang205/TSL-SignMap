using Microsoft.EntityFrameworkCore;
using FeedbackService.Data;
using FeedbackService.Models;
using FeedbackService.DTOs;
using FeedbackService.Mapper;
using System.Net.Http.Json;

namespace FeedbackService.Services
{
    /// Service quản lý Feedbacks theo requirement
    /// - Users can submit feedback or report issues with the app or database
    /// - Reporting inappropriate content or misuse is supported
    /// - Admins can review and manage feedback status (Pending, Reviewed, Resolved)
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FeedbackService(FeedbackDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;

            var userServiceUrl = _configuration["ServiceEndpoints:UserService"] 
                ?? throw new InvalidOperationException("UserService endpoint not configured");
            _httpClient.BaseAddress = new Uri(userServiceUrl);
        }

        /// Lấy feedback theo ID
        public async Task<FeedbackDto> GetByIdAsync(int id)
        {
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
                throw new InvalidOperationException("Feedback not found");

            // Lấy username từ UserService
            var username = await GetUsernameFromUserServiceAsync(feedback.UserId);

            return feedback.ToDto(username);
        }

        /// Lấy tất cả feedbacks của một user
        public async Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            // Lấy username từ UserService
            var username = await GetUsernameFromUserServiceAsync(userId);

            return feedbacks.Select(f => f.ToDto(username));
        }

        /// Lấy tất cả feedbacks theo status
        public async Task<IEnumerable<FeedbackDto>> GetByStatusAsync(string status)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.Status == status)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            // Lấy usernames từ UserService cho tất cả unique user IDs
            var userIds = feedbacks.Select(f => f.UserId).Distinct().ToList();
            var usernameMap = new Dictionary<int, string?>();

            foreach (var userId in userIds)
            {
                usernameMap[userId] = await GetUsernameFromUserServiceAsync(userId);
            }

            return feedbacks.Select(f => f.ToDto(usernameMap.GetValueOrDefault(f.UserId)));
        }

        /// Tạo feedback mới
        /// Validation: Kiểm tra user tồn tại qua UserService
        public async Task<FeedbackDto> CreateAsync(FeedbackCreateRequest request)
        {
            // Kiểm tra user tồn tại qua UserService
            await ValidateUserExistsAsync(request.UserId);

            // Tạo feedback mới
            var feedback = new Feedback
            {
                UserId = request.UserId,
                Content = request.Content,
                Status = "Pending", // Default status
                CreatedAt = DateTime.UtcNow,
                ResolvedAt = null
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Lấy username từ UserService
            var username = await GetUsernameFromUserServiceAsync(feedback.UserId);

            return feedback.ToDto(username);
        }

        /// Cập nhật feedback (content và status)
        public async Task<FeedbackDto> UpdateAsync(int id, FeedbackUpdateRequest request)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                throw new InvalidOperationException("Feedback not found");

            // Cập nhật các trường nếu có
            if (request.Content != null)
            {
                feedback.Content = request.Content;
            }

            if (request.Status != null)
            {
                feedback.Status = request.Status;
                
                // Nếu status là "Resolved" và ResolvedAt chưa được set, tự động set
                if (request.Status == "Resolved" && feedback.ResolvedAt == null)
                {
                    feedback.ResolvedAt = DateTime.UtcNow;
                }
                // Nếu status không phải "Resolved" và ResolvedAt đã được set, clear nó
                else if (request.Status != "Resolved" && feedback.ResolvedAt != null)
                {
                    feedback.ResolvedAt = null;
                }
            }

            await _context.SaveChangesAsync();

            // Lấy username từ UserService
            var username = await GetUsernameFromUserServiceAsync(feedback.UserId);

            return feedback.ToDto(username);
        }

        /// Cập nhật status của feedback
        /// AutoResolve: Nếu true và status = "Resolved", tự động set ResolvedAt
        public async Task<FeedbackDto> UpdateStatusAsync(int id, FeedbackStatusUpdateRequest request)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                throw new InvalidOperationException("Feedback not found");

            feedback.Status = request.Status;

            // Xử lý ResolvedAt
            if (request.Status == "Resolved" && request.AutoResolve && feedback.ResolvedAt == null)
            {
                feedback.ResolvedAt = DateTime.UtcNow;
            }
            else if (request.Status != "Resolved" && feedback.ResolvedAt != null)
            {
                feedback.ResolvedAt = null;
            }

            await _context.SaveChangesAsync();

            // Lấy username từ UserService
            var username = await GetUsernameFromUserServiceAsync(feedback.UserId);

            return feedback.ToDto(username);
        }

        /// Xóa feedback
        public async Task<bool> DeleteAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                throw new InvalidOperationException("Feedback not found");

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return true;
        }

        /// Filter feedbacks với các điều kiện
        public async Task<IEnumerable<FeedbackDto>> FilterAsync(FeedbackFilterRequest request)
        {
            var query = _context.Feedbacks
                .AsQueryable();

            // Filter theo UserId
            if (request.UserId.HasValue)
            {
                query = query.Where(f => f.UserId == request.UserId.Value);
            }

            // Filter theo Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(f => f.Status == request.Status);
            }

            // Filter theo IsResolved (ResolvedAt != null)
            if (request.IsResolved.HasValue)
            {
                if (request.IsResolved.Value)
                {
                    query = query.Where(f => f.ResolvedAt != null);
                }
                else
                {
                    query = query.Where(f => f.ResolvedAt == null);
                }
            }

            // Filter theo date range
            if (request.StartDate.HasValue)
            {
                query = query.Where(f => f.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(f => f.CreatedAt <= request.EndDate.Value);
            }

            // Filter theo search (Content)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower();
                query = query.Where(f => f.Content.ToLower().Contains(searchLower));
            }

            // Sắp xếp theo thời gian mới nhất
            query = query.OrderByDescending(f => f.CreatedAt);

            // Pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var feedbacks = await query.ToListAsync();

            // Lấy usernames từ UserService cho tất cả unique user IDs
            var userIds = feedbacks.Select(f => f.UserId).Distinct().ToList();
            var usernameMap = new Dictionary<int, string?>();

            foreach (var userId in userIds)
            {
                usernameMap[userId] = await GetUsernameFromUserServiceAsync(userId);
            }

            return feedbacks.Select(f => f.ToDto(usernameMap.GetValueOrDefault(f.UserId)));
        }

        /// Lấy tổng hợp feedbacks
        /// Bao gồm: Total feedbacks, Pending, Reviewed, Resolved, Average resolution time
        public async Task<FeedbackSummaryResponse> GetSummaryAsync()
        {
            var feedbacks = await _context.Feedbacks
                .ToListAsync();

            var resolvedFeedbacks = feedbacks.Where(f => f.ResolvedAt != null).ToList();
            var averageResolutionTime = resolvedFeedbacks.Count > 0
                ? resolvedFeedbacks
                    .Select(f => (f.ResolvedAt!.Value - f.CreatedAt).TotalDays)
                    .Average()
                : 0;

            var summary = new FeedbackSummaryResponse
            {
                TotalFeedbacks = feedbacks.Count,
                PendingFeedbacks = feedbacks.Count(f => f.Status == "Pending"),
                ReviewedFeedbacks = feedbacks.Count(f => f.Status == "Reviewed"),
                ResolvedFeedbacks = feedbacks.Count(f => f.Status == "Resolved"),
                AverageResolutionTime = averageResolutionTime
            };

            return summary;
        }

        // Helper methods để gọi UserService
        private async Task ValidateUserExistsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"User {userId} not found: {ex.Message}", ex);
            }
        }

        private async Task<string?> GetUsernameFromUserServiceAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserResponse>();
                    return user?.Username;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

