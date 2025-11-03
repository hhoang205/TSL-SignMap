using System.Threading.Tasks;
using WebAppTrafficSign.Services.Interfaces;

namespace WebAppTrafficSign.Services
{
    public class EmailService : IEmailService
    {
        // Tạm thời implementation đơn giản, sau này có thể dùng SendGrid, MailKit...
        public async Task SendWelcomeEmailAsync(string toEmail, string username)
        {
            // TODO: Implement gửi email thật
            await Task.CompletedTask;
            Console.WriteLine($"Welcome email sent to {toEmail} for user {username}");
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            // TODO: Implement gửi email thật
            await Task.CompletedTask;
            Console.WriteLine($"Password reset email sent to {toEmail} with link {resetLink}");
        }
    }
}