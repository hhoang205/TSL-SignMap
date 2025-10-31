using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GeneratePasswordResetToken(User user);
    }

    public interface ICoinWalletService
    {
        Task CreateWalletAsync(int userId, decimal initialBalance);
        Task<decimal> GetBalanceAsync(int userId);
        Task<bool> CreditAsync(int userId, decimal amount);
        Task<bool> DebitAsync(int userId, decimal amount);
        Task<bool> HasEnoughBalanceAsync(int userId, decimal amount);
    }

    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string username);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
