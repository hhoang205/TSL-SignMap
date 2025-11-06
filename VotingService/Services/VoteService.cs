using Microsoft.EntityFrameworkCore;
using VotingService.Data;
using VotingService.Models;
using VotingService.DTOs;
using VotingService.Mapper;
using System.Text.Json;

namespace VotingService.Services
{
    /// Interface cho VoteService
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

    /// Service quản lý Votes theo requirement
    /// - Users can vote on contributions (upvote/downvote)
    /// - Votes có weight (1.0 for full vote, có thể giảm dần)
    /// - Mỗi user chỉ có thể vote 1 lần cho mỗi contribution (unique constraint)
    /// - Validate User và Contribution tồn tại qua HTTP calls
    public class VoteService : IVoteService
    {
        private readonly VoteDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VoteService(
            VoteDbContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// Lấy vote theo ID
        public async Task<VoteDto> GetByIdAsync(int id)
        {
            var vote = await _context.Votes.FindAsync(id);

            if (vote == null)
                throw new InvalidOperationException("Vote not found");

            return vote.ToDto();
        }

        /// Lấy tất cả votes của một contribution
        public async Task<IEnumerable<VoteDto>> GetByContributionIdAsync(int contributionId)
        {
            var votes = await _context.Votes
                .Where(v => v.ContributionId == contributionId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return votes.Select(v => v.ToDto());
        }

        /// Lấy tất cả votes của một user
        public async Task<IEnumerable<VoteDto>> GetByUserIdAsync(int userId)
        {
            var votes = await _context.Votes
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return votes.Select(v => v.ToDto());
        }

        /// Tạo vote mới
        /// Validation: Kiểm tra user và contribution tồn tại, và user chưa vote cho contribution này
        public async Task<VoteDto> CreateAsync(VoteCreateRequest request)
        {
            // Validate User tồn tại (via HTTP call to UserService)
            await ValidateUserExistsAsync(request.UserId);

            // Validate Contribution tồn tại (via HTTP call to ContributionService)
            await ValidateContributionExistsAsync(request.ContributionId);

            // Kiểm tra user chưa vote cho contribution này
            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(v => v.ContributionId == request.ContributionId && v.UserId == request.UserId);

            if (existingVote != null)
                throw new InvalidOperationException("User has already voted for this contribution");

            // Tạo vote mới
            var vote = new Vote
            {
                ContributionId = request.ContributionId,
                UserId = request.UserId,
                Value = request.Value ? 1 : -1,
                IsUpvote = request.Value,
                Weight = request.Weight,
                CreatedAt = DateTime.UtcNow
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return vote.ToDto();
        }

        /// Cập nhật vote
        public async Task<VoteDto> UpdateAsync(int id, VoteUpdateRequest request)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
                throw new InvalidOperationException("Vote not found");

            // Cập nhật các trường nếu có
            if (request.Value.HasValue)
            {
                vote.Value = request.Value.Value ? 1 : -1;
                vote.IsUpvote = request.Value.Value;
            }

            if (request.Weight.HasValue)
            {
                vote.Weight = request.Weight.Value;
            }

            await _context.SaveChangesAsync();

            return vote.ToDto();
        }

        /// Xóa vote
        public async Task<bool> DeleteAsync(int id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
                throw new InvalidOperationException("Vote not found");

            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();

            return true;
        }

        /// Filter votes với các điều kiện
        public async Task<IEnumerable<VoteDto>> FilterAsync(VoteFilterRequest request)
        {
            var query = _context.Votes.AsQueryable();

            // Filter theo ContributionId
            if (request.ContributionId.HasValue)
            {
                query = query.Where(v => v.ContributionId == request.ContributionId.Value);
            }

            // Filter theo UserId
            if (request.UserId.HasValue)
            {
                query = query.Where(v => v.UserId == request.UserId.Value);
            }

            // Filter theo Value (upvote/downvote)
            if (request.Value.HasValue)
            {
                query = query.Where(v => v.IsUpvote == request.Value.Value);
            }

            // Filter theo date range
            if (request.StartDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt <= request.EndDate.Value);
            }

            // Sắp xếp theo thời gian mới nhất
            query = query.OrderByDescending(v => v.CreatedAt);

            // Pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            var votes = await query.ToListAsync();
            return votes.Select(v => v.ToDto());
        }

        /// Lấy tổng hợp votes của một contribution
        /// Bao gồm: Total votes, Upvotes, Downvotes, Average weight, Total score
        public async Task<VoteSummaryResponse> GetVoteSummaryAsync(int contributionId)
        {
            // Validate Contribution tồn tại (via HTTP call to ContributionService)
            await ValidateContributionExistsAsync(contributionId);

            var votes = await _context.Votes
                .Where(v => v.ContributionId == contributionId)
                .ToListAsync();

            var summary = new VoteSummaryResponse
            {
                ContributionId = contributionId,
                TotalVotes = votes.Count,
                Upvotes = votes.Count(v => v.IsUpvote),
                Downvotes = votes.Count(v => !v.IsUpvote),
                AverageWeight = votes.Count > 0 ? votes.Average(v => v.Weight) : 0,
                TotalScore = votes.Sum(v => v.Value * v.Weight)
            };

            return summary;
        }

        /// Validate User tồn tại qua HTTP call
        private async Task ValidateUserExistsAsync(int userId)
        {
            var userServiceUrl = _configuration["ServiceEndpoints:UserService"] ?? "http://localhost:5001";
            var response = await _httpClient.GetAsync($"{userServiceUrl}/api/users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new InvalidOperationException("User not found");
                throw new InvalidOperationException($"Failed to validate user: {response.StatusCode}");
            }
        }

        /// Validate Contribution tồn tại qua HTTP call
        private async Task ValidateContributionExistsAsync(int contributionId)
        {
            var contributionServiceUrl = _configuration["ServiceEndpoints:ContributionService"] ?? "http://localhost:5003";
            var response = await _httpClient.GetAsync($"{contributionServiceUrl}/api/contributions/{contributionId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new InvalidOperationException("Contribution not found");
                throw new InvalidOperationException($"Failed to validate contribution: {response.StatusCode}");
            }
        }
    }
}

