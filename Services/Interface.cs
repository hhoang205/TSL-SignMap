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
    }

    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string username);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
