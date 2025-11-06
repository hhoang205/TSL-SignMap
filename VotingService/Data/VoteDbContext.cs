using Microsoft.EntityFrameworkCore;
using VotingService.Models;

namespace VotingService.Data
{
    public class VoteDbContext : DbContext
    {
        public VoteDbContext(DbContextOptions<VoteDbContext> options) : base(options)
        {
        }

        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Vote>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Value).IsRequired();
                entity.Property(v => v.Weight).IsRequired();
                entity.Property(v => v.CreatedAt).IsRequired();

                // Tránh người dùng bỏ phiếu nhiều lần cho cùng một góp ý
                entity.HasIndex(v => new { v.ContributionId, v.UserId }).IsUnique();
            });
        }
    }
}

