using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebAppTrafficSign.Models;

namespace WebAppTrafficSign.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CoinWallet> CoinWallets { get; set; }
        public DbSet<TrafficSign> TrafficSigns { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User table
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);
                entity.Property(u => u.Password)
                      .IsRequired();
                entity.Property(u => u.RoleId)
                      .IsRequired();
                entity.Property(u => u.CreatedAt)
                      .IsRequired();

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // One-to-one with CoinWallet (configured on CoinWallet)
            });

            // CoinWallet table
            builder.Entity<CoinWallet>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Balance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(c => c.CreatedAt)
                      .IsRequired();

                // One-to-one relationship with User
                entity.HasOne<User>()
                      .WithOne()
                      .HasForeignKey<CoinWallet>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UserId).IsUnique();
            });

            // TrafficSign table
            builder.Entity<TrafficSign>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Type)
                      .IsRequired();
                entity.Property(t => t.Location)
                      .HasColumnType("geography")
                      .HasColumnType("geography").HasAnnotation("Srid", 4326)
                      .IsRequired();
                entity.Property(t => t.Status)
                      .HasMaxLength(50);
                entity.Property(t => t.ValidFrom)
                      .IsRequired();
            });

            // Contribution table
            builder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Action)
                      .IsRequired();
                entity.Property(c => c.Status)
                      .IsRequired();
                entity.Property(c => c.CreatedAt)
                      .IsRequired();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<TrafficSign>()
                      .WithMany()
                      .HasForeignKey(c => c.SignId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Vote table
            builder.Entity<Vote>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Value)
                      .IsRequired();
                entity.Property(v => v.Weight)
                      .IsRequired();
                entity.Property(v => v.CreatedAt)
                      .IsRequired();

                entity.HasOne<Contribution>()
                      .WithMany()
                      .HasForeignKey(v => v.ContributionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Tránh người dùng bỏ phiếu nhiều lần cho cùng một góp ý
                entity.HasIndex(v => new { v.ContributionId, v.UserId }).IsUnique();
            });

            // Notification table
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Title)
                      .IsRequired();
                entity.Property(n => n.Message)
                      .IsRequired();
                entity.Property(n => n.CreatedAt)
                      .IsRequired();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Feedback table
            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Content)
                      .IsRequired();
                entity.Property(f => f.Status)
                      .HasMaxLength(50);
                entity.Property(f => f.CreatedAt)
                      .IsRequired();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
