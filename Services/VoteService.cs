using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using WebAppTrafficSign.Mapper;

namespace WebAppTrafficSign.Services
{
    /// Service quản lý Votes theo requirement
    /// - Users can vote on contributions (upvote/downvote)
    /// - Votes có weight (1.0 for full vote, có thể giảm dần)
    /// - Mỗi user chỉ có thể vote 1 lần cho mỗi contribution (unique constraint)
    public class VoteService : IVoteService
    {
        private readonly ApplicationDbContext _context;

        public VoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Lấy vote theo ID
        public async Task<VoteDto> GetByIdAsync(int id)
        {
            var vote = await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Contribution)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vote == null)
                throw new InvalidOperationException("Vote not found");

            return vote.toDto();
        }

        /// Lấy tất cả votes của một contribution
        public async Task<IEnumerable<VoteDto>> GetByContributionIdAsync(int contributionId)
        {
            var votes = await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Contribution)
                .Where(v => v.ContributionId == contributionId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return votes.Select(v => v.toDto());
        }

        /// Lấy tất cả votes của một user
        public async Task<IEnumerable<VoteDto>> GetByUserIdAsync(int userId)
        {
            var votes = await _context.Votes
                .Include(v => v.User)
                .Include(v => v.Contribution)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return votes.Select(v => v.toDto());
        }

        /// Tạo vote mới
        /// Validation: Kiểm tra user và contribution tồn tại, và user chưa vote cho contribution này
        public async Task<VoteDto> CreateAsync(VoteCreateRequest request)
        {
            // Kiểm tra contribution tồn tại
            var contribution = await _context.Contributions.FindAsync(request.ContributionId);
            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

            // Kiểm tra user tồn tại
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

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
                CreatedAt = DateTime.Now
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            // Load lại với includes
            await _context.Entry(vote).Reference(v => v.User).LoadAsync();
            await _context.Entry(vote).Reference(v => v.Contribution).LoadAsync();

            return vote.toDto();
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

            // Load lại với includes
            await _context.Entry(vote).Reference(v => v.User).LoadAsync();
            await _context.Entry(vote).Reference(v => v.Contribution).LoadAsync();

            return vote.toDto();
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
            var query = _context.Votes
                .Include(v => v.User)
                .Include(v => v.Contribution)
                .AsQueryable();

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
            return votes.Select(v => v.toDto());
        }

        /// Lấy tổng hợp votes của một contribution
        /// Bao gồm: Total votes, Upvotes, Downvotes, Average weight, Total score
        public async Task<VoteSummaryResponse> GetVoteSummaryAsync(int contributionId)
        {
            // Kiểm tra contribution tồn tại
            var contribution = await _context.Contributions.FindAsync(contributionId);
            if (contribution == null)
                throw new InvalidOperationException("Contribution not found");

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
    }
}

