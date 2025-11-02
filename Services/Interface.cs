using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;

namespace WebAppTrafficSign.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GeneratePasswordResetToken(User user);
    }

    public interface ICoinWalletService
    {
        Task<CoinWalletDto> GetByIdAsync(Guid id);
        Task<CoinWalletDto> GetByUserIdAsync(int userId);
        Task<decimal> GetBalanceAsync(int userId);
        Task<CoinWalletDto> CreateAsync(CoinWalletCreateRequest request);
        Task<CoinWalletDto> UpdateAsync(Guid id, CoinWalletUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CoinWalletDto>> FilterAsync(CoinWalletFilterRequest request);
        Task<CoinWalletSummaryResponse> GetSummaryAsync();
        Task<bool> CreditAsync(int userId, decimal amount);
        Task<bool> DebitAsync(int userId, decimal amount);
        Task<bool> HasEnoughBalanceAsync(int userId, decimal amount);
        Task<CoinWalletDto> AdjustAsync(int userId, CoinWalletAdjustRequest request);
    }

    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string username);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }

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

    public interface IVoteService
    {
        Task<VoteDto> GetByIdAsync(int id);
        Task<IEnumerable<VoteDto>> GetByContributionIdAsync(int contributionId);
        Task<IEnumerable<VoteDto>> GetByUserIdAsync(int userId);
        Task<VoteDto> CreateAsync(VoteCreateRequest request);
        Task<VoteDto> UpdateAsync(int id, VoteUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VoteDto>> FilterAsync(VoteFilterRequest request);
        Task<VoteSummaryResponse> GetVoteSummaryAsync(int contributionId);
    }

    public interface IFeedbackService
    {
        Task<FeedbackDto> GetByIdAsync(int id);
        Task<IEnumerable<FeedbackDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<FeedbackDto>> GetByStatusAsync(string status);
        Task<FeedbackDto> CreateAsync(FeedbackCreateRequest request);
        Task<FeedbackDto> UpdateAsync(int id, FeedbackUpdateRequest request);
        Task<FeedbackDto> UpdateStatusAsync(int id, FeedbackStatusUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FeedbackDto>> FilterAsync(FeedbackFilterRequest request);
        Task<FeedbackSummaryResponse> GetSummaryAsync();
    }

    public interface IPaymentService
    {
        Task<PaymentDto> GetByIdAsync(int id);
        Task<IEnumerable<PaymentDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<PaymentDto>> GetByStatusAsync(string status);
        Task<PaymentDto> CreateAsync(PaymentCreateRequest request);
        Task<PaymentDto> UpdateAsync(int id, PaymentUpdateRequest request);
        Task<PaymentDto> UpdateStatusAsync(int id, PaymentStatusUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PaymentDto>> FilterAsync(PaymentFilterRequest request);
        Task<PaymentSummaryResponse> GetSummaryAsync();
    }
}
