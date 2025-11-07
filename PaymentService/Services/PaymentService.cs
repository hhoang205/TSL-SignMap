using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Models;
using PaymentService.Mapper;

namespace PaymentService.Services
{
    /// Triển khai PaymentService sử dụng Entity Framework Core để thao tác dữ liệu.
    /// Cung cấp các phương thức để tạo, cập nhật, đọc và xóa giao dịch nạp tiền,
    /// đồng thời cập nhật số dư trong ví xu của người dùng khi thanh toán hoàn tất.
    public class PaymentService : IPaymentService
    {
        private readonly PaymentDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(PaymentDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            
            var userServiceUrl = _configuration["ServiceEndpoints:UserService"] ?? "http://localhost:5001";
            _httpClient.BaseAddress = new Uri(userServiceUrl);
        }

        /// Lấy thông tin giao dịch theo Id.
        public async Task<PaymentDto> GetByIdAsync(int id)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (payment == null)
                throw new InvalidOperationException("Payment not found");
            
            // Lấy username từ UserService
            string username = await GetUsernameFromUserServiceAsync(payment.UserId);
            
            return payment.ToDto(username);
        }

        /// Lấy tất cả payments của một user
        public async Task<IEnumerable<PaymentDto>> GetByUserIdAsync(int userId)
        {
            var payments = await _context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .AsNoTracking()
                .ToListAsync();

            // Lấy username từ UserService
            string username = await GetUsernameFromUserServiceAsync(userId);

            return payments.Select(p => p.ToDto(username));
        }

        /// Lấy tất cả payments theo status
        public async Task<IEnumerable<PaymentDto>> GetByStatusAsync(string status)
        {
            var payments = await _context.Payments
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .AsNoTracking()
                .ToListAsync();

            // Lấy usernames từ UserService cho tất cả unique user IDs
            var userIds = payments.Select(p => p.UserId).Distinct().ToList();
            var usernameMap = await GetUsernamesFromUserServiceAsync(userIds);

            return payments.Select(p => p.ToDto(usernameMap.GetValueOrDefault(p.UserId, "")));
        }

        /// Tạo giao dịch thanh toán mới. Khi thanh toán thành công, số xu được cộng vào ví của người dùng.
        public async Task<PaymentDto> CreateAsync(PaymentCreateRequest request)
        {
            // Sử dụng transaction để đảm bảo atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.Amount <= 0m) throw new ArgumentException("Số tiền phải lớn hơn 0");

                // Kiểm tra người dùng tồn tại qua UserService
                var userExists = await ValidateUserExistsAsync(request.UserId);
                if (!userExists)
                    throw new InvalidOperationException("User not found");

                var payment = new Payment
                {
                    UserId = request.UserId,
                    Amount = request.Amount,
                    PaymentDate = request.PaymentDate ?? DateTime.UtcNow,
                    PaymentMethod = request.PaymentMethod,
                    Status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status
                };

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                // Cập nhật ví xu của người dùng nếu trạng thái là Completed (gọi UserService)
                if (payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    await CreditCoinsToUserAsync(payment.UserId, payment.Amount);
                }

                await transaction.CommitAsync();
                
                // Lấy username từ UserService
                string username = await GetUsernameFromUserServiceAsync(payment.UserId);
                
                return payment.ToDto(username);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                if (ex.InnerException != null)
                {
                    throw new Exception($"Lỗi khi tạo payment: {ex.Message}. Chi tiết: {ex.InnerException.Message}", ex);
                }
                throw;
            }
        }

        /// Cập nhật giao dịch thanh toán. Nếu thay đổi số tiền hoặc trạng thái từ Pending sang Completed, 
        /// số xu trong ví sẽ được điều chỉnh tương ứng.
        public async Task<PaymentDto> UpdateAsync(int id, PaymentUpdateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null) throw new InvalidOperationException("Payment not found");

                // Lưu số tiền và trạng thái cũ để so sánh
                var oldAmount = payment.Amount;
                var oldStatus = payment.Status;

                // Cập nhật các trường
                if (request.Amount.HasValue && request.Amount.Value > 0m)
                {
                    payment.Amount = request.Amount.Value;
                }
                if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
                {
                    payment.PaymentMethod = request.PaymentMethod;
                }
                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    payment.Status = request.Status;
                }
                if (request.PaymentDate.HasValue)
                {
                    payment.PaymentDate = request.PaymentDate.Value;
                }

                // Lưu thay đổi
                await _context.SaveChangesAsync();

                // Nếu trạng thái mới là Completed nhưng trước đó không phải Completed, cộng xu
                if (!oldStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                    payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    await CreditCoinsToUserAsync(payment.UserId, payment.Amount);
                }
                // Nếu thay đổi số tiền trong trạng thái Completed, điều chỉnh chênh lệch
                else if (oldStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                         payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                         payment.Amount != oldAmount)
                {
                    var difference = payment.Amount - oldAmount;
                    if (difference > 0)
                        await CreditCoinsToUserAsync(payment.UserId, difference);
                    else
                        await DebitCoinsFromUserAsync(payment.UserId, Math.Abs(difference));
                }

                await transaction.CommitAsync();
                
                // Lấy username từ UserService
                string username = await GetUsernameFromUserServiceAsync(payment.UserId);
                
                return payment.ToDto(username);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// Cập nhật status của payment
        public async Task<PaymentDto> UpdateStatusAsync(int id, PaymentStatusUpdateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null) throw new InvalidOperationException("Payment not found");

                var oldStatus = payment.Status;
                payment.Status = request.Status;

                await _context.SaveChangesAsync();

                // Nếu trạng thái mới là Completed nhưng trước đó không phải Completed, cộng xu
                if (!oldStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                    payment.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    await CreditCoinsToUserAsync(payment.UserId, payment.Amount);
                }

                await transaction.CommitAsync();
                
                // Lấy username từ UserService
                string username = await GetUsernameFromUserServiceAsync(payment.UserId);
                
                return payment.ToDto(username);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// Xóa giao dịch thanh toán. Lưu ý: không hoàn xu khi xóa, bạn có thể điều chỉnh theo nghiệp vụ.
        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                throw new InvalidOperationException("Payment not found");

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        /// Filter payments với các điều kiện
        public async Task<IEnumerable<PaymentDto>> FilterAsync(PaymentFilterRequest request)
        {
            var query = _context.Payments
                .AsQueryable();

            // Filter theo UserId
            if (request.UserId.HasValue)
            {
                query = query.Where(p => p.UserId == request.UserId.Value);
            }

            // Filter theo Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                query = query.Where(p => p.Status == request.Status);
            }

            // Filter theo PaymentMethod
            if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
            {
                query = query.Where(p => p.PaymentMethod == request.PaymentMethod);
            }

            // Filter theo date range
            if (request.StartDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= request.EndDate.Value);
            }

            // Filter theo amount range
            if (request.MinAmount.HasValue)
            {
                query = query.Where(p => p.Amount >= request.MinAmount.Value);
            }

            if (request.MaxAmount.HasValue)
            {
                query = query.Where(p => p.Amount <= request.MaxAmount.Value);
            }

            // Sắp xếp theo thời gian mới nhất
            query = query.OrderByDescending(p => p.PaymentDate);

            // Pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var payments = await query.AsNoTracking().ToListAsync();

            // Lấy usernames từ UserService cho tất cả unique user IDs
            var userIds = payments.Select(p => p.UserId).Distinct().ToList();
            var usernameMap = await GetUsernamesFromUserServiceAsync(userIds);

            return payments.Select(p => p.ToDto(usernameMap.GetValueOrDefault(p.UserId, "")));
        }

        /// Lấy tổng hợp payments
        /// Bao gồm: Total payments, Completed, Pending, Failed, Total amount, Average amount, Total completed amount
        public async Task<PaymentSummaryResponse> GetSummaryAsync()
        {
            var payments = await _context.Payments
                .AsNoTracking()
                .ToListAsync();

            var completedPayments = payments.Where(p => p.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)).ToList();
            
            var summary = new PaymentSummaryResponse
            {
                TotalPayments = payments.Count,
                CompletedPayments = payments.Count(p => p.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)),
                PendingPayments = payments.Count(p => p.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
                FailedPayments = payments.Count(p => p.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase)),
                TotalAmount = payments.Sum(p => p.Amount),
                AverageAmount = payments.Count > 0 ? payments.Average(p => p.Amount) : 0,
                TotalCompletedAmount = completedPayments.Sum(p => p.Amount)
            };

            return summary;
        }

        // Helper methods để gọi UserService

        private async Task<bool> ValidateUserExistsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetUsernameFromUserServiceAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserResponse>();
                    return user?.Username ?? "";
                }
            }
            catch
            {
                // Ignore errors, return empty string
            }
            return "";
        }

        private async Task<Dictionary<int, string>> GetUsernamesFromUserServiceAsync(List<int> userIds)
        {
            var usernameMap = new Dictionary<int, string>();
            
            // Gọi parallel để lấy tất cả usernames
            var tasks = userIds.Select(async userId =>
            {
                var username = await GetUsernameFromUserServiceAsync(userId);
                return new { UserId = userId, Username = username };
            });

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                usernameMap[result.UserId] = result.Username;
            }

            return usernameMap;
        }

        private async Task CreditCoinsToUserAsync(int userId, decimal amount)
        {
            try
            {
                var request = new CreditRequest { Amount = amount };
                var response = await _httpClient.PostAsJsonAsync($"/api/wallets/user/{userId}/credit", request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to credit coins to user {userId}: {ex.Message}", ex);
            }
        }

        private async Task DebitCoinsFromUserAsync(int userId, decimal amount)
        {
            try
            {
                var request = new DebitRequest { Amount = amount };
                var response = await _httpClient.PostAsJsonAsync($"/api/wallets/user/{userId}/debit", request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to debit coins from user {userId}: {ex.Message}", ex);
            }
        }
    }
}

