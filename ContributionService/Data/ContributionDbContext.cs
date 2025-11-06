using Microsoft.EntityFrameworkCore;
using ContributionService.Models;

namespace ContributionService.Data
{
    public class ContributionDbContext : DbContext
    {
        public ContributionDbContext(DbContextOptions<ContributionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contribution> Contributions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Action).IsRequired();
                entity.Property(c => c.Status).IsRequired();
                entity.Property(c => c.CreatedAt).IsRequired();
            });
        }
    }
}

