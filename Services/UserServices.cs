using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Mapper;

namespace WebAppTrafficSign.Services
{
    /// Định nghĩa các chức năng của UserService
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegistrationRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> UpdateProfileAsync(int id, UserUpdateRequest request);
        Task<bool> ChangePasswordAsync(int id, ChangePasswordRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
    /// Lớp triển khai UserService chứa toàn bộ nghiệp vụ người dùng
    /// </summary>
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ICoinWalletService _coinWalletService;
        private readonly IEmailService _emailService;

        public UserService(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService,
            ICoinWalletService coinWalletService,
            IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _coinWalletService = coinWalletService;
            _emailService = emailService;
        }
        /// Đăng ký tài khoản mới
        /// </summary>
        public async Task<UserDto> RegisterAsync(UserRegistrationRequest request)
        {
            // Kiểm tra tài khoản tồn tại
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
                throw new InvalidOperationException("Tên đăng nhập hoặc email đã tồn tại.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = 0, // 0: user
                Reputation = 0f,
                CreatedAt = DateTime.UtcNow
            };

            user.Password = _passwordHasher.HashPassword(user, request.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Tạo ví xu ban đầu 20 coin
            await _coinWalletService.CreateWalletAsync(user.Id, 20m);

            // Gửi email chào mừng (nếu cần)
            await _emailService.SendWelcomeEmailAsync(user.Email, user.Username);

            return user.toDto();
        }

        /// <summary>
        /// Đăng nhập, trả về token xác thực
        /// </summary>
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new InvalidOperationException("Tài khoản không tồn tại.");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verifyResult != PasswordVerificationResult.Success)
                throw new InvalidOperationException("Mật khẩu không đúng.");

            var token = _tokenService.GenerateToken(user);
            return new AuthResponse { User = user.ToDto(), Token = token };
        }

        /// <summary>
        /// Lấy thông tin người dùng theo Id
        /// </summary>
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user?.ToDto();
        }

        /// <summary>
        /// Cập nhật hồ sơ người dùng (tên, email, phone)
        /// </summary>
        public async Task<UserDto> UpdateProfileAsync(int id, UserUpdateRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new InvalidOperationException("User not found");

            // Kiểm tra email hoặc username mới có bị trùng không
            if (await _context.Users.AnyAsync(u => (u.Email == request.Email || u.Username == request.Username) && u.Id != id))
                throw new InvalidOperationException("Tên đăng nhập hoặc email đã được sử dụng.");

            user.Username = request.Username;
            user.Email = request.Email;
            user.Phone = request.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return user.ToDto();
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new InvalidOperationException("User not found");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (verifyResult != PasswordVerificationResult.Success)
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Yêu cầu đặt lại mật khẩu (gửi email)
        /// </summary>
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            var token = _tokenService.GeneratePasswordResetToken(user);
            var resetLink = request.CallbackUrl + "?token=" + token;
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
            return true;
        }

        /// <summary>
        /// Xóa hoặc vô hiệu hóa người dùng
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
    

