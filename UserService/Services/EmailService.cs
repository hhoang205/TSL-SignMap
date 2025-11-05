namespace UserService.Services
{
    /// Service gửi email (mock implementation - trong production cần tích hợp với email service như SendGrid, SMTP, etc.)
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string username);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        /// Gửi email chào mừng khi user đăng ký thành công
        public async Task SendWelcomeEmailAsync(string toEmail, string username)
        {
            // TODO: Trong production, tích hợp với email service thực tế (SendGrid, SMTP, etc.)
            _logger.LogInformation($"Sending welcome email to {toEmail} for user {username}");
            
            // Mock: Chỉ log ra, không gửi email thực tế
            await Task.CompletedTask;
        }

        /// Gửi email reset password với link
        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            // TODO: Trong production, tích hợp với email service thực tế
            _logger.LogInformation($"Sending password reset email to {toEmail} with link: {resetLink}");
            
            // Mock: Chỉ log ra, không gửi email thực tế
            await Task.CompletedTask;
        }
    }
}

