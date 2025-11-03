using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GeneratePasswordResetToken(User user);
        bool ValidateToken(string token, string expectedUsername);
        string ExtractUsername(string token);
        string ExtractRole(string token);

        string ExtractUserId(string token);
        bool IsTokenExpired(string token);

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
