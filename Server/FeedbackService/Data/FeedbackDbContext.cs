using Microsoft.EntityFrameworkCore;
using FeedbackService.Models;

namespace FeedbackService.Data
{
    public class FeedbackDbContext : DbContext
    {
        public FeedbackDbContext(DbContextOptions<FeedbackDbContext> options) : base(options)
        {
        }

        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Feedback configuration
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Content).IsRequired();
                entity.Property(f => f.Status).HasMaxLength(50);
                entity.Property(f => f.CreatedAt).IsRequired();

                // Indexes
                entity.HasIndex(f => f.UserId);
                entity.HasIndex(f => f.Status);
                entity.HasIndex(f => f.CreatedAt);
            });
        }
    }
}

