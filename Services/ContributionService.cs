using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Mapper;
using NetTopologySuite.Geometries;

namespace WebAppTrafficSign.Services
{
    /// Interface cho ContributionService
    public interface IContributionService
    {
        Task<IEnumerable<ContributionDto>> GetAllAsync();
        Task<ContributionDto> GetByIdAsync(int id);
        Task<IEnumerable<ContributionDto>> GetByStatusAsync(string status);
        Task<IEnumerable<ContributionDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<ContributionDto>> FilterAsync(ContributionFilterRequest request);
        Task<ContributionDto> SubmitAsync(ContributionCreateRequest request);
        Task<ContributionDto> ApproveAsync(int contributionId, ContributionReviewRequest request);
        Task<ContributionDto> RejectAsync(int contributionId, ContributionReviewRequest request);
        Task<ContributionDto> UpdateAsync(int id, ContributionCreateRequest request);
        Task<bool> DeleteAsync(int id);
    }

    /// Service quản lý Contributions theo requirement
    /// - Users submit contributions (tốn 5 coins)
    /// - Admin approve/reject contributions
    /// - Approved contributions -> convert to TrafficSign + reward coins (10+)
    /// - Rejected contributions -> notify user
    public class ContributionService : IContributionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICoinWalletService _coinWalletService;
        private readonly ITrafficSignService _trafficSignService;
        private readonly INotificationService _notificationService;

        private const decimal SubmissionCost = 5m;
        private const decimal ApprovalReward = 10m;

        public ContributionService(
            ApplicationDbContext context,
            ICoinWalletService coinWalletService,
            ITrafficSignService trafficSignService,
            INotificationService notificationService)
        {
            _context = context;
            _coinWalletService = coinWalletService;
            _trafficSignService = trafficSignService;
            _notificationService = notificationService;
        }

        /// Lấy tất cả contributions
        public async Task<IEnumerable<ContributionDto>> GetAllAsync()
        {
            var contributions = await _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Lấy contribution theo ID
        public async Task<ContributionDto> GetByIdAsync(int id)
        {
            var contribution = await _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            return contribution.toDto();
        }

        /// Lấy contributions theo status
        public async Task<IEnumerable<ContributionDto>> GetByStatusAsync(string status)
        {
            var contributions = await _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Lấy contributions theo userId
        public async Task<IEnumerable<ContributionDto>> GetByUserIdAsync(int userId)
        {
            var contributions = await _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Filter contributions theo các criteria
        public async Task<IEnumerable<ContributionDto>> FilterAsync(ContributionFilterRequest request)
        {
            IQueryable<Contribution> query = _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(c => c.Status == request.Status);
            }

            if (!string.IsNullOrWhiteSpace(request.Action))
            {
                query = query.Where(c => c.Action == request.Action);
            }

            if (request.UserId.HasValue)
            {
                query = query.Where(c => c.UserId == request.UserId.Value);
            }

            var contributions = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Submit contribution mới (tốn 5 coins)
        public async Task<ContributionDto> SubmitAsync(ContributionCreateRequest request)
        {
            // Validate request
            if (request.Action == "Add")
            {
                if (string.IsNullOrWhiteSpace(request.Type))
                    throw new ArgumentException("Type is required for Add action");
                if (!request.Latitude.HasValue || !request.Longitude.HasValue)
                    throw new ArgumentException("Latitude and Longitude are required for Add action");
            }
            else if (request.Action == "Update" || request.Action == "Delete")
            {
                if (!request.SignId.HasValue)
                    throw new ArgumentException("SignId is required for Update/Delete actions");
                
                // Verify TrafficSign exists
                var trafficSign = await _context.TrafficSigns.FindAsync(request.SignId.Value);
                if (trafficSign == null)
                    throw new InvalidOperationException("TrafficSign not found");
            }
            else
            {
                throw new ArgumentException("Invalid Action. Must be 'Add', 'Update', or 'Delete'");
            }

            // Charge 5 coins for submission
            await ChargeCoinForSubmissionAsync(request.UserId);

            // Create contribution
            var contribution = new Contribution
            {
                UserId = request.UserId,
                Action = request.Action,
                Description = request.Description ?? string.Empty,
                ImageUrl = request.ImageUrl ?? string.Empty,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Type = request.Type ?? string.Empty,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                SignId = request.SignId ?? 0,
                TrafficSignId = request.SignId ?? 0
            };

            await _context.Contributions.AddAsync(contribution);
            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(contribution)
                .Reference(c => c.User).LoadAsync();
            await _context.Entry(contribution)
                .Reference(c => c.TrafficSign).LoadAsync();

            return contribution.toDto();
        }

        /// Admin approve contribution (reward 10+ coins, convert to TrafficSign if Action = "Add")
        public async Task<ContributionDto> ApproveAsync(int contributionId, ContributionReviewRequest request)
        {
            var contribution = await _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign)
                .FirstOrDefaultAsync(c => c.Id == contributionId);

            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            if (contribution.Status != "Pending")
                throw new InvalidOperationException("Contribution is not pending");

            contribution.Status = "Approved";

            // Reward coins to user
            decimal rewardAmount = request.RewardAmount ?? ApprovalReward;
            await _coinWalletService.CreditAsync(contribution.UserId, rewardAmount);

            // Process based on action
            if (contribution.Action == "Add")
            {
                // Create new TrafficSign from contribution
                if (contribution.Latitude.HasValue && contribution.Longitude.HasValue && !string.IsNullOrWhiteSpace(contribution.Type))
                {
                    var trafficSignCreateRequest = new TrafficSignCreateRequest
                    {
                        Type = contribution.Type,
                        Latitude = contribution.Latitude.Value,
                        Longitude = contribution.Longitude.Value,
                        Status = "Active",
                        ImageUrl = contribution.ImageUrl,
                        ValidFrom = DateTime.UtcNow
                    };

                    var newTrafficSign = await _trafficSignService.CreateAsync(trafficSignCreateRequest);
                    
                    // Update contribution with created TrafficSign ID
                    contribution.SignId = newTrafficSign.Id;
                    contribution.TrafficSignId = newTrafficSign.Id;
                }
            }
            else if (contribution.Action == "Update")
            {
                // Update existing TrafficSign
                if (contribution.SignId > 0)
                {
                    var updateRequest = new TrafficSignUpdateRequest
                    {
                        Type = !string.IsNullOrWhiteSpace(contribution.Type) ? contribution.Type : null,
                        Latitude = contribution.Latitude,
                        Longitude = contribution.Longitude,
                        ImageUrl = !string.IsNullOrWhiteSpace(contribution.ImageUrl) ? contribution.ImageUrl : null
                    };

                    await _trafficSignService.UpdateAsync(contribution.SignId, updateRequest);
                }
            }
            else if (contribution.Action == "Delete")
            {
                // Delete TrafficSign
                if (contribution.SignId > 0)
                {
                    await _trafficSignService.DeleteAsync(contribution.SignId);
                }
            }

            await _context.SaveChangesAsync();

            // Send notification to user
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                UserId = contribution.UserId,
                Title = "Contribution Approved",
                Message = $"Your contribution #{contributionId} has been approved. You received {rewardAmount} coins reward."
            });

            return contribution.toDto();
        }

        /// Admin reject contribution (notify user)
        public async Task<ContributionDto> RejectAsync(int contributionId, ContributionReviewRequest request)
        {
            var contribution = await _context.Contributions
                .Include(c => c.User)
                .Include(c => c.TrafficSign)
                .FirstOrDefaultAsync(c => c.Id == contributionId);

            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            if (contribution.Status != "Pending")
                throw new InvalidOperationException("Contribution is not pending");

            contribution.Status = "Rejected";
            await _context.SaveChangesAsync();

            // Send notification to user
            string message = $"Your contribution #{contributionId} has been rejected.";
            if (!string.IsNullOrWhiteSpace(request.AdminNote))
            {
                message += $" Reason: {request.AdminNote}";
            }
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                UserId = contribution.UserId,
                Title = "Contribution Rejected",
                Message = message
            });

            return contribution.toDto();
        }

        /// Update contribution (only if status is Pending and user owns it)
        public async Task<ContributionDto> UpdateAsync(int id, ContributionCreateRequest request)
        {
            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            if (contribution.Status != "Pending")
                throw new InvalidOperationException("Can only update pending contributions");

            if (contribution.UserId != request.UserId)
                throw new UnauthorizedAccessException("You can only update your own contributions");

            contribution.Action = request.Action;
            contribution.Description = request.Description ?? contribution.Description;
            contribution.ImageUrl = request.ImageUrl ?? contribution.ImageUrl;
            contribution.Type = request.Type ?? contribution.Type;
            contribution.Latitude = request.Latitude ?? contribution.Latitude;
            contribution.Longitude = request.Longitude ?? contribution.Longitude;
            contribution.SignId = request.SignId ?? contribution.SignId;
            contribution.TrafficSignId = contribution.SignId;

            await _context.SaveChangesAsync();

            // Load navigation properties
            await _context.Entry(contribution)
                .Reference(c => c.User).LoadAsync();
            await _context.Entry(contribution)
                .Reference(c => c.TrafficSign).LoadAsync();

            return contribution.toDto();
        }

        /// Delete contribution (only if status is Pending and user owns it)
        public async Task<bool> DeleteAsync(int id)
        {
            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
                return false;

            if (contribution.Status != "Pending")
                throw new InvalidOperationException("Can only delete pending contributions");

            _context.Contributions.Remove(contribution);
            await _context.SaveChangesAsync();
            return true;
        }

        /// Charge 5 coins for submission
        private async Task<bool> ChargeCoinForSubmissionAsync(int userId)
        {
            if (!await _coinWalletService.HasEnoughBalanceAsync(userId, SubmissionCost))
            {
                throw new InvalidOperationException($"Không đủ coin để submit contribution. Cần {SubmissionCost} coin.");
            }

            await _coinWalletService.DebitAsync(userId, SubmissionCost);
            return true;
        }

    }
}

