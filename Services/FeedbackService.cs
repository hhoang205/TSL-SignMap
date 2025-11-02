using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Mapper;

namespace WebAppTrafficSign.Services
{
    /// Service quản lý Feedbacks theo requirement
    /// - Users can submit feedback or report issues with the app or database
    /// - Reporting inappropriate content or misuse is supported
    /// - Admins can review and manage feedback status (Pending, Reviewed, Resolved)
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Lấy feedback theo ID
        public async Task<FeedbackDto> GetByIdAsync(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
                throw new InvalidOperationException("Feedback not found");

            return feedback.toDto();
        }

        /// Lấy tất cả feedbacks của một user
        public async Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks.Select(f => f.toDto());
        }

        /// Lấy tất cả feedbacks theo status
        public async Task<IEnumerable<FeedbackDto>> GetByStatusAsync(string status)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.Status == status)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks.Select(f => f.toDto());
        }

        /// Tạo feedback mới
        /// Validation: Kiểm tra user tồn tại
        public async Task<FeedbackDto> CreateAsync(FeedbackCreateRequest request)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

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

            // Load lại với includes
            await _context.Entry(feedback).Reference(f => f.User).LoadAsync();

            return feedback.toDto();
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

            // Load lại với includes
            await _context.Entry(feedback).Reference(f => f.User).LoadAsync();

            return feedback.toDto();
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

            // Load lại với includes
            await _context.Entry(feedback).Reference(f => f.User).LoadAsync();

            return feedback.toDto();
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
                .Include(f => f.User)
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
            return feedbacks.Select(f => f.toDto());
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
    }
}

