using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Mapper;
using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.Data;
using Microsoft.EntityFrameworkCore;

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
        Task<WalletBalanceResponse> GetWalletBalanceAsync(int id);
        Task<TopUpCoinsResponse> TopUpCoinsAsync(int userId, CoinTopUpRequest request);
        Task<UserProfileResponse> GetUserProfileAsync(int id);
    }
    /// Lớp triển khai UserService chứa toàn bộ nghiệp vụ người dùng
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ICoinWalletService _coinWalletService;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;

        public UserService(
            ApplicationDbContext context,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService,
            ICoinWalletService coinWalletService,
            IEmailService emailService,
            IPaymentService paymentService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _coinWalletService = coinWalletService;
            _emailService = emailService;
            _paymentService = paymentService;
        }
        /// Đăng ký tài khoản mới
        public async Task<UserDto> RegisterAsync(UserRegistrationRequest request)
        {
            // Sử dụng transaction để đảm bảo atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra tài khoản tồn tại
                if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
                    throw new InvalidOperationException("Tên đăng nhập hoặc email đã tồn tại.");

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Firstname = request.Username, // Required by DB, use Username as default
                    Lastname = "User", // Required by DB, set default value
                    RoleId = 0, // 0: user
                    Reputation = 0f,
                    CreatedAt = DateTime.UtcNow
                };

                user.Password = _passwordHasher.HashPassword(user, request.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // Tạo ví xu ban đầu 20 coin - tạo trực tiếp trong cùng context để tránh tracking issues
                var wallet = new CoinWallet
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Balance = 20m,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.CoinWallets.AddAsync(wallet);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Gửi email chào mừng (nếu cần) - không cần trong transaction
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.Username);
                }
                catch
                {
                    // Ignore email errors, không ảnh hưởng đến registration
                }

                return user.toDto();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log inner exception nếu có
                if (ex.InnerException != null)
                {
                    throw new Exception($"Lỗi khi đăng ký: {ex.Message}. Chi tiết: {ex.InnerException.Message}", ex);
                }
                throw;
            }
        }

        /// Đăng nhập, trả về token xác thực
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new InvalidOperationException("Tài khoản không tồn tại.");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (verifyResult != PasswordVerificationResult.Success)
                throw new InvalidOperationException("Mật khẩu không đúng.");

            var token = _tokenService.GenerateToken(user);
            return new AuthResponse { User = user.toDto(), Token = token };
        }

        /// Lấy thông tin người dùng theo Id
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");
            return user.toDto();
        }

        /// Cập nhật hồ sơ người dùng (tên, email, phone)
        public async Task<UserDto> UpdateProfileAsync(int id, UserUpdateRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new InvalidOperationException("User not found");

            // Kiểm tra email hoặc username mới có bị trùng không
            if (await _context.Users.AnyAsync(u => (u.Email == request.Email || u.Username == request.Username) && u.Id != id))
                throw new InvalidOperationException("Tên đăng nhập hoặc email đã được sử dụng.");

            user.Username = request.Username;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return user.toDto();
        }

        /// Đổi mật khẩu
        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new InvalidOperationException("User not found");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword);
            if (verifyResult != PasswordVerificationResult.Success)
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");

            user.Password = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        /// Yêu cầu đặt lại mật khẩu (gửi email)
        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            var token = _tokenService.GeneratePasswordResetToken(user);
            var resetLink = request.CallbackUrl + "?token=" + token;
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
            return true;
        }

        /// Xóa hoặc vô hiệu hóa người dùng
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }


        /// Lấy số dư coin trong ví của user
        public async Task<WalletBalanceResponse> GetWalletBalanceAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var balance = await _coinWalletService.GetBalanceAsync(id);
            return new WalletBalanceResponse
            {
                UserId = id,
                Balance = balance,
                Username = user.Username
            };
        }


        /// Nạp coin vào ví (tích hợp với Payment service)
        /// Theo requirement: $1 = 10 Coins
        public async Task<TopUpCoinsResponse> TopUpCoinsAsync(int userId, CoinTopUpRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Tính số coin dựa trên số tiền: $1 = 10 Coins
            const decimal coinRate = 10m; // 10 coins per dollar
            decimal coinsToAdd = request.AmountInDollars * coinRate;

            if (coinsToAdd <= 0)
                throw new ArgumentException("Số tiền phải lớn hơn 0");

            // Tạo payment record
            var paymentRequest = new PaymentCreateRequest
            {
                UserId = userId,
                Amount = coinsToAdd,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod ?? "Credit Card",
                Status = "Completed" // Giả sử thanh toán thành công ngay lập tức
            };

            var payment = await _paymentService.CreateAsync(paymentRequest);

            // Lấy số dư mới sau khi nạp
            var newBalance = await _coinWalletService.GetBalanceAsync(userId);

            return new TopUpCoinsResponse
            {
                UserId = userId,
                AmountInDollars = request.AmountInDollars,
                CoinsAdded = coinsToAdd,
                NewBalance = newBalance,
                PaymentId = payment.Id,
                PaymentDate = payment.PaymentDate
            };
        }


        /// Lấy thông tin đầy đủ profile của user bao gồm coin balance
        public async Task<UserProfileResponse> GetUserProfileAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Wallet)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new InvalidOperationException("User not found");

            var balance = await _coinWalletService.GetBalanceAsync(id);

            return new UserProfileResponse
            {
                User = user.toDto(),
                CoinBalance = balance,
                TotalContributions = user.Contributions?.Count ?? 0,
                TotalVotes = user.Votes?.Count ?? 0
            };
        }
    }
}
    

