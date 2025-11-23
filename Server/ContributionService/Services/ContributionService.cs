using Microsoft.EntityFrameworkCore;
using ContributionService.Data;
using ContributionService.Models;
using ContributionService.DTOs;
using ContributionService.Mapper;
using System.Text.Json;

namespace ContributionService.Services
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
    /// - Users submit contributions (tốn 5 coins) - gọi UserService
    /// - Admin approve/reject contributions
    /// - Approved contributions -> convert to TrafficSign + reward coins (10+) - gọi TrafficSignService và UserService
    /// - Rejected contributions -> notify user - gọi NotificationService
    public class ContributionService : IContributionService
    {
        private readonly ContributionDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private const decimal SubmissionCost = 5m;
        private const decimal ApprovalReward = 10m;

        public ContributionService(
            ContributionDbContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// Lấy tất cả contributions
        public async Task<IEnumerable<ContributionDto>> GetAllAsync()
        {
            var contributions = await _context.Contributions
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Lấy contribution theo ID
        public async Task<ContributionDto> GetByIdAsync(int id)
        {
            var contribution = await _context.Contributions.FindAsync(id);

            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            return contribution.toDto();
        }

        /// Lấy contributions theo status
        public async Task<IEnumerable<ContributionDto>> GetByStatusAsync(string status)
        {
            var contributions = await _context.Contributions
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Lấy contributions theo userId
        public async Task<IEnumerable<ContributionDto>> GetByUserIdAsync(int userId)
        {
            var contributions = await _context.Contributions
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contributions.Select(c => c.toDto());
        }

        /// Filter contributions theo các criteria
        public async Task<IEnumerable<ContributionDto>> FilterAsync(ContributionFilterRequest request)
        {
            IQueryable<Contribution> query = _context.Contributions;

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

        /// Submit contribution mới (tốn 5 coins) - gọi UserService để debit
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
            }
            else
            {
                throw new ArgumentException("Invalid Action. Must be 'Add', 'Update', or 'Delete'");
            }

            // Charge 5 coins for submission (via HTTP call to UserService)
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

            return contribution.toDto();
        }

        /// Admin approve contribution (reward 10+ coins, convert to TrafficSign if Action = "Add")
        /// Gọi TrafficSignService để tạo/update/delete sign
        /// Gọi UserService để credit coins
        /// Gọi NotificationService để notify user
        public async Task<ContributionDto> ApproveAsync(int contributionId, ContributionReviewRequest request)
        {
            var contribution = await _context.Contributions.FindAsync(contributionId);

            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            if (contribution.Status != "Pending")
                throw new InvalidOperationException("Contribution is not pending");

            contribution.Status = "Approved";

            // Reward coins to user (via HTTP call to UserService)
            decimal rewardAmount = request.RewardAmount ?? ApprovalReward;
            await CreditCoinsAsync(contribution.UserId, rewardAmount);

            // Process based on action (via HTTP call to TrafficSignService)
            if (contribution.Action == "Add")
            {
                // Create new TrafficSign from contribution
                if (contribution.Latitude.HasValue && contribution.Longitude.HasValue && !string.IsNullOrWhiteSpace(contribution.Type))
                {
                    var trafficSignCreateRequest = new
                    {
                        Type = contribution.Type,
                        Latitude = contribution.Latitude.Value,
                        Longitude = contribution.Longitude.Value,
                        Status = "Active",
                        ImageUrl = contribution.ImageUrl,
                        ValidFrom = DateTime.UtcNow
                    };

                    var trafficSignServiceUrl = _configuration["ServiceEndpoints:TrafficSignService"] ?? "http://localhost:5002";
                    var response = await _httpClient.PostAsJsonAsync($"{trafficSignServiceUrl}/api/signs", trafficSignCreateRequest);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                        if (result.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var id))
                        {
                            contribution.SignId = id.GetInt32();
                            contribution.TrafficSignId = contribution.SignId;
                        }
                    }
                }
            }
            else if (contribution.Action == "Update")
            {
                // Update existing TrafficSign
                if (contribution.SignId > 0)
                {
                    var trafficSignUpdateRequest = new
                    {
                        Type = !string.IsNullOrWhiteSpace(contribution.Type) ? contribution.Type : (string?)null,
                        Latitude = contribution.Latitude,
                        Longitude = contribution.Longitude,
                        ImageUrl = !string.IsNullOrWhiteSpace(contribution.ImageUrl) ? contribution.ImageUrl : (string?)null
                    };

                    var trafficSignServiceUrl = _configuration["ServiceEndpoints:TrafficSignService"] ?? "http://localhost:5002";
                    await _httpClient.PutAsJsonAsync($"{trafficSignServiceUrl}/api/signs/{contribution.SignId}", trafficSignUpdateRequest);
                }
            }
            else if (contribution.Action == "Delete")
            {
                // Delete TrafficSign
                if (contribution.SignId > 0)
                {
                    var trafficSignServiceUrl = _configuration["ServiceEndpoints:TrafficSignService"] ?? "http://localhost:5002";
                    await _httpClient.DeleteAsync($"{trafficSignServiceUrl}/api/signs/{contribution.SignId}");
                }
            }

            await _context.SaveChangesAsync();

            // Send notification to user (via HTTP call to NotificationService)
            await SendNotificationAsync(contribution.UserId, "Contribution Approved", 
                $"Your contribution #{contributionId} has been approved. You received {rewardAmount} coins reward.");

            return contribution.toDto();
        }

        /// Admin reject contribution (notify user) - gọi NotificationService
        public async Task<ContributionDto> RejectAsync(int contributionId, ContributionReviewRequest request)
        {
            var contribution = await _context.Contributions.FindAsync(contributionId);

            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            if (contribution.Status != "Pending")
                throw new InvalidOperationException("Contribution is not pending");

            contribution.Status = "Rejected";
            await _context.SaveChangesAsync();

            // Send notification to user (via HTTP call to NotificationService)
            string message = $"Your contribution #{contributionId} has been rejected.";
            if (!string.IsNullOrWhiteSpace(request.AdminNote))
            {
                message += $" Reason: {request.AdminNote}";
            }
            await SendNotificationAsync(contribution.UserId, "Contribution Rejected", message);

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

        /// Charge 5 coins for submission (via HTTP call to UserService)
        private async Task<bool> ChargeCoinForSubmissionAsync(int userId)
        {
            try
            {
                var userServiceUrl = _configuration["ServiceEndpoints:UserService"] ?? "http://localhost:5001";
                var checkBalanceUrl = $"{userServiceUrl}/api/wallets/user/{userId}/check-balance";
                var debitUrl = $"{userServiceUrl}/api/wallets/user/{userId}/debit";

                // Check balance
                var checkRequest = new { Amount = SubmissionCost };
                var checkResponse = await _httpClient.PostAsJsonAsync(checkBalanceUrl, checkRequest);
                
                if (!checkResponse.IsSuccessStatusCode)
                {
                    // Try to extract error message from response
                    string errorMessage = $"Không đủ coin để submit contribution. Cần {SubmissionCost} coin.";
                    try
                    {
                        var errorContent = await checkResponse.Content.ReadFromJsonAsync<JsonElement>();
                        if (errorContent.TryGetProperty("message", out var messageProp))
                        {
                            errorMessage = messageProp.GetString() ?? errorMessage;
                        }
                        else if (errorContent.TryGetProperty("Message", out var messagePropPascal))
                        {
                            errorMessage = messagePropPascal.GetString() ?? errorMessage;
                        }
                    }
                    catch
                    {
                        // Use default message if parsing fails
                    }
                    throw new InvalidOperationException(errorMessage);
                }

                var checkResult = await checkResponse.Content.ReadFromJsonAsync<JsonElement>();
                // Try PascalCase first (default C# JSON serialization), then camelCase
                bool hasEnough = false;
                bool propertyFound = false;
                
                if (checkResult.TryGetProperty("HasEnoughBalance", out var hasEnoughPascal))
                {
                    hasEnough = hasEnoughPascal.GetBoolean();
                    propertyFound = true;
                }
                else if (checkResult.TryGetProperty("hasEnoughBalance", out var hasEnoughCamel))
                {
                    hasEnough = hasEnoughCamel.GetBoolean();
                    propertyFound = true;
                }
                
                // If property not found, throw error instead of defaulting to false
                if (!propertyFound)
                {
                    throw new InvalidOperationException($"Lỗi kiểm tra số dư: Không thể đọc kết quả từ UserService. Vui lòng thử lại.");
                }
                
                if (!hasEnough)
                {
                    throw new InvalidOperationException($"Không đủ coin để submit contribution. Cần {SubmissionCost} coin.");
                }

                // Debit coin
                var debitRequest = new { Amount = SubmissionCost };
                var debitResponse = await _httpClient.PostAsJsonAsync(debitUrl, debitRequest);
                
                if (!debitResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Không thể trừ coin. Vui lòng thử lại.");
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Lỗi kết nối với UserService: {ex.Message}");
            }
        }

        /// Credit coins to user (via HTTP call to UserService)
        private async Task<bool> CreditCoinsAsync(int userId, decimal amount)
        {
            try
            {
                var userServiceUrl = _configuration["ServiceEndpoints:UserService"] ?? "http://localhost:5001";
                var creditUrl = $"{userServiceUrl}/api/wallets/user/{userId}/credit";
                
                var request = new { Amount = amount };
                var response = await _httpClient.PostAsJsonAsync(creditUrl, request);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Không thể cộng coin. Vui lòng thử lại.");
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Lỗi kết nối với UserService: {ex.Message}");
            }
        }

        /// Send notification (via HTTP call to NotificationService)
        private async Task<bool> SendNotificationAsync(int userId, string title, string message)
        {
            try
            {
                var notificationServiceUrl = _configuration["ServiceEndpoints:NotificationService"] ?? "http://localhost:5005";
                var createUrl = $"{notificationServiceUrl}/api/notifications";
                
                var request = new { UserId = userId, Title = title, Message = message };
                var response = await _httpClient.PostAsJsonAsync(createUrl, request);
                
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                // Log error but don't fail the operation
                return false;
            }
        }
    }
}

