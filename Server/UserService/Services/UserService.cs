using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.DTOs;
using UserService.Mapper;

namespace UserService.Services
{
    /// Định nghĩa các chức năng của UserService
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegistrationRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserDto> GetByIdAsync(int id);
        Task<PaginatedUsersResponse> GetAllAsync(int pageNumber, int pageSize, string? username = null);
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
        private readonly UserDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICoinWalletService _coinWalletService;
        private readonly IEmailService _emailService;
        // TODO: Add HTTP client for PaymentService communication
        // private readonly HttpClient _httpClient;

        public UserService(
            UserDbContext context,
            IPasswordHasher<User> passwordHasher,
            ICoinWalletService coinWalletService,
            IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _coinWalletService = coinWalletService;
            _emailService = emailService;
        }

        /// Đăng ký tài khoản mới
        public async Task<UserDto> RegisterAsync(UserRegistrationRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
                    throw new InvalidOperationException("Tên đăng nhập hoặc email đã tồn tại.");

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Firstname = request.Username,
                    Lastname = "User",
                    RoleId = 0, // 0: user
                    Reputation = 0f,
                    CreatedAt = DateTime.UtcNow
                };

                user.Password = _passwordHasher.HashPassword(user, request.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // Tạo ví xu ban đầu 20 coin
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

                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.Username);
                }
                catch
                {
                    // Ignore email errors
                }

                return user.toDto();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

            return new AuthResponse 
            { 
                User = user.toDto(),
                Message = "Đăng nhập thành công bằng Firebase token."
            };
        }

        /// Lấy thông tin người dùng theo Id
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");
            return user.toDto();
        }

        /// Lấy danh sách users với pagination và filter
        public async Task<PaginatedUsersResponse> GetAllAsync(int pageNumber, int pageSize, string? username = null)
        {
            var query = _context.Users.AsQueryable();

            // Filter by username if provided
            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(u => u.Username.Contains(username) || u.Email.Contains(username));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => u.toDto())
                .ToListAsync();

            return new PaginatedUsersResponse
            {
                Data = users,
                Count = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        /// Cập nhật hồ sơ người dùng (tên, email, phone, role)
        public async Task<UserDto> UpdateProfileAsync(int id, UserUpdateRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new InvalidOperationException("User not found");

            // Check for duplicate email/username if they're being updated
            if (!string.IsNullOrWhiteSpace(request.Email) || !string.IsNullOrWhiteSpace(request.Username))
            {
                var email = request.Email ?? user.Email;
                var username = request.Username ?? user.Username;
                if (await _context.Users.AnyAsync(u => (u.Email == email || u.Username == username) && u.Id != id))
                    throw new InvalidOperationException("Tên đăng nhập hoặc email đã được sử dụng.");
            }

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(request.Username))
                user.Username = request.Username;
            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(request.Firstname))
                user.Firstname = request.Firstname;
            if (!string.IsNullOrWhiteSpace(request.Lastname))
                user.Lastname = request.Lastname;
            
            // Update role if provided (convert string to RoleId)
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                user.RoleId = request.Role switch
                {
                    "Staff" => 1,
                    "Admin" => 2,
                    _ => 0  // Default to User
                };
            }

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

            var rawToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var token = rawToken.TrimEnd('=').Replace('+', '-').Replace('/', '_');
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
        /// TODO: Implement HTTP client call to PaymentService
        public async Task<TopUpCoinsResponse> TopUpCoinsAsync(int userId, CoinTopUpRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Tính số coin dựa trên số tiền: $1 = 10 Coins
            const decimal coinRate = 10m;
            decimal coinsToAdd = request.AmountInDollars * coinRate;

            if (coinsToAdd <= 0)
                throw new ArgumentException("Số tiền phải lớn hơn 0");

            // TODO: Call PaymentService via HTTP to create payment record
            // For now, directly credit the wallet
            await _coinWalletService.CreditAsync(userId, coinsToAdd);

            var newBalance = await _coinWalletService.GetBalanceAsync(userId);

            return new TopUpCoinsResponse
            {
                UserId = userId,
                AmountInDollars = request.AmountInDollars,
                CoinsAdded = coinsToAdd,
                NewBalance = newBalance,
                PaymentId = 0, // TODO: Get from PaymentService response
                PaymentDate = DateTime.UtcNow
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

            // TODO: Call other services (ContributionService, VotingService) via HTTP to get counts
            return new UserProfileResponse
            {
                User = user.toDto(),
                CoinBalance = balance,
                TotalContributions = 0, // TODO: Get from ContributionService
                TotalVotes = 0 // TODO: Get from VotingService
            };
        }
    }
}

