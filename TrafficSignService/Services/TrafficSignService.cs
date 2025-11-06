using Microsoft.EntityFrameworkCore;
using TrafficSignService.Data;
using TrafficSignService.Models;
using TrafficSignService.DTOs;
using TrafficSignService.Mapper;
using NetTopologySuite.Geometries;

namespace TrafficSignService.Services
{
    /// Interface cho TrafficSignService
    public interface ITrafficSignService
    {
        Task<IEnumerable<TrafficSignDto>> GetAllAsync();
        Task<TrafficSignDto> GetByIdAsync(int id);
        Task<IEnumerable<TrafficSignDto>> SearchAsync(TrafficSignSearchRequest request);
        Task<IEnumerable<TrafficSignDto>> FilterByProximityAsync(TrafficSignProximityFilterRequest request);
        Task<IEnumerable<TrafficSignDto>> FilterByTypeAsync(string type);
        Task<TrafficSignDto> CreateAsync(TrafficSignCreateRequest request);
        Task<TrafficSignDto> UpdateAsync(int id, TrafficSignUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }

    /// Service quản lý Traffic Signs theo requirement
    /// - Hiển thị real-time traffic sign locations
    /// - Search và filter với coin checking (qua HTTP call to UserService)
    public class TrafficSignService : ITrafficSignService
    {
        private readonly TrafficSignDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TrafficSignService(
            TrafficSignDbContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// Lấy tất cả traffic signs (hiển thị trên map) - không tốn coin
        public async Task<IEnumerable<TrafficSignDto>> GetAllAsync()
        {
            var signs = await _context.TrafficSigns
                .Where(s => s.Status == "Active" && s.ValidTo >= DateTime.UtcNow)
                .ToListAsync();

            return signs.Select(s => s.toDto());
        }

        /// Lấy traffic sign theo ID
        public async Task<TrafficSignDto> GetByIdAsync(int id)
        {
            var sign = await _context.TrafficSigns.FindAsync(id);
            if (sign == null)
                throw new InvalidOperationException("Traffic sign not found");

            return sign.toDto();
        }

        /// Tìm kiếm traffic signs theo type hoặc location - tốn 1 coin cho advanced filters
        public async Task<IEnumerable<TrafficSignDto>> SearchAsync(TrafficSignSearchRequest request)
        {
            IQueryable<TrafficSign> query = _context.TrafficSigns
                .Where(s => s.Status == "Active" && s.ValidTo >= DateTime.UtcNow);

            // Nếu có type filter - đây là advanced filter, cần charge coin
            bool isAdvancedFilter = !string.IsNullOrWhiteSpace(request.Type) || 
                                    (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue);

            if (isAdvancedFilter && request.UserId.HasValue)
            {
                // Charge 1 coin cho advanced filter (via HTTP call to UserService)
                await ChargeCoinForAdvancedFilterAsync(request.UserId.Value);
            }

            // Filter by type
            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                query = query.Where(s => s.Type.Contains(request.Type));
            }

            // Filter by location and radius (proximity)
            if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
            {
                var centerPoint = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 };
                var radiusInMeters = request.RadiusKm.Value * 1000; // Convert km to meters

                query = query.Where(s => 
                    s.Location.Distance(centerPoint) <= radiusInMeters);
            }

            var signs = await query.ToListAsync();
            return signs.Select(s => s.toDto());
        }

        /// Filter theo proximity (bán kính) - tốn 1 coin
        public async Task<IEnumerable<TrafficSignDto>> FilterByProximityAsync(TrafficSignProximityFilterRequest request)
        {
            if (request.UserId.HasValue)
            {
                await ChargeCoinForAdvancedFilterAsync(request.UserId.Value);
            }

            var centerPoint = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
            var radiusInMeters = request.RadiusKm * 1000; // Convert km to meters

            var signs = await _context.TrafficSigns
                .Where(s => s.Status == "Active" && 
                           s.ValidTo >= DateTime.UtcNow &&
                           s.Location.Distance(centerPoint) <= radiusInMeters)
                .ToListAsync();

            return signs.Select(s => s.toDto());
        }

        /// Filter theo type - tốn 1 coin
        public async Task<IEnumerable<TrafficSignDto>> FilterByTypeAsync(string type)
        {
            var signs = await _context.TrafficSigns
                .Where(s => s.Status == "Active" && 
                           s.ValidTo >= DateTime.UtcNow &&
                           s.Type.Contains(type))
                .ToListAsync();

            return signs.Select(s => s.toDto());
        }

        /// Tạo traffic sign mới (dành cho admin hoặc từ approved contribution)
        public async Task<TrafficSignDto> CreateAsync(TrafficSignCreateRequest request)
        {
            var sign = new TrafficSign
            {
                Type = request.Type,
                Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 },
                Status = request.Status ?? "Active",
                ImageUrl = request.ImageUrl ?? string.Empty,
                ValidFrom = request.ValidFrom ?? DateTime.UtcNow,
                ValidTo = request.ValidTo ?? DateTime.MaxValue
            };

            await _context.TrafficSigns.AddAsync(sign);
            await _context.SaveChangesAsync();

            return sign.toDto();
        }

        /// Cập nhật traffic sign
        public async Task<TrafficSignDto> UpdateAsync(int id, TrafficSignUpdateRequest request)
        {
            var sign = await _context.TrafficSigns.FindAsync(id);
            if (sign == null)
                throw new InvalidOperationException("Traffic sign not found");

            sign.Type = request.Type ?? sign.Type;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                sign.Location = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 };
            }
            sign.Status = request.Status ?? sign.Status;
            sign.ImageUrl = request.ImageUrl ?? sign.ImageUrl;
            if (request.ValidFrom.HasValue)
                sign.ValidFrom = request.ValidFrom.Value;
            if (request.ValidTo.HasValue)
                sign.ValidTo = request.ValidTo.Value;

            await _context.SaveChangesAsync();
            return sign.toDto();
        }

        /// Xóa traffic sign
        public async Task<bool> DeleteAsync(int id)
        {
            var sign = await _context.TrafficSigns.FindAsync(id);
            if (sign == null)
                return false;

            _context.TrafficSigns.Remove(sign);
            await _context.SaveChangesAsync();
            return true;
        }

        /// Charge 1 coin cho advanced filter/search (via HTTP call to UserService)
        private async Task<bool> ChargeCoinForAdvancedFilterAsync(int userId)
        {
            const decimal filterCost = 1m;
            
            try
            {
                var userServiceUrl = _configuration["ServiceEndpoints:UserService"] ?? "http://localhost:5001";
                var checkBalanceUrl = $"{userServiceUrl}/api/wallets/user/{userId}/check-balance";
                var debitUrl = $"{userServiceUrl}/api/wallets/user/{userId}/debit";

                // Check balance
                var checkRequest = new { Amount = filterCost };
                var checkResponse = await _httpClient.PostAsJsonAsync(checkBalanceUrl, checkRequest);
                
                if (!checkResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Không đủ coin để sử dụng tính năng tìm kiếm nâng cao. Cần {filterCost} coin.");
                }

                var checkResult = await checkResponse.Content.ReadFromJsonAsync<dynamic>();
                if (checkResult?.GetProperty("hasEnoughBalance").GetBoolean() == false)
                {
                    throw new InvalidOperationException($"Không đủ coin để sử dụng tính năng tìm kiếm nâng cao. Cần {filterCost} coin.");
                }

                // Debit coin
                var debitRequest = new { Amount = filterCost };
                var debitResponse = await _httpClient.PostAsJsonAsync(debitUrl, debitRequest);
                
                if (!debitResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Không thể trừ coin. Vui lòng thử lại.");
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Lỗi kết nối với UserService: {ex.Message}");
            }
        }
    }
}

