using Microsoft.EntityFrameworkCore;
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
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================
            // User
            // =========================
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.RoleId).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // 1–1: User ↔ CoinWallet (Wallet là dependent, FK ở CoinWallet.UserId)
                entity.HasOne(u => u.Wallet)
                      .WithOne(w => w.User)
                      .HasForeignKey<CoinWallet>(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // CoinWallet
            // =========================
            builder.Entity<CoinWallet>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Balance)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(c => c.CreatedAt).IsRequired();

                // Đảm bảo quan hệ 1–1
                entity.HasIndex(c => c.UserId).IsUnique();
            });

            // =========================
            // TrafficSign
            // =========================
            builder.Entity<TrafficSign>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Type).IsRequired();
                entity.Property(t => t.Location)
                      .HasColumnType("geography")
                      .IsRequired();
                entity.Property(t => t.Status).HasMaxLength(50);
                entity.Property(t => t.ValidFrom).IsRequired();
            });

            // =========================
            // Contribution
            // =========================
            builder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Action).IsRequired();
                entity.Property(c => c.Status).IsRequired();
                entity.Property(c => c.CreatedAt).IsRequired();

                // Contribution -> User (N-1)
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Contributions)   // nếu chưa có property, có thể đổi thành .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Contribution -> TrafficSign (N-1)
                entity.HasOne(c => c.TrafficSign)
                      .WithMany(t => t.Contributions)   // nếu chưa có property, có thể đổi thành .WithMany()
                      .HasForeignKey(c => c.SignId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // Vote
            // =========================
            builder.Entity<Vote>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Value).IsRequired();
                entity.Property(v => v.Weight).IsRequired();
                entity.Property(v => v.CreatedAt).IsRequired();

                // Vote -> Contribution (N-1) GIỮ Cascade
                entity.HasOne(v => v.Contribution)
                      .WithMany(c => c.Votes)           // nếu chưa có property, có thể đổi thành .WithMany()
                      .HasForeignKey(v => v.ContributionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Vote -> User (N-1) ĐỔI sang Restrict để tránh multiple cascade paths
                entity.HasOne(v => v.User)
                      .WithMany(u => u.Votes)           // nếu chưa có property, có thể đổi thành .WithMany()
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Tránh người dùng bỏ phiếu nhiều lần cho cùng một góp ý
                entity.HasIndex(v => new { v.ContributionId, v.UserId }).IsUnique();
            });

            // =========================
            // Notification
            // =========================
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.Title).IsRequired();
                entity.Property(n => n.Message).IsRequired();
                entity.Property(n => n.CreatedAt).IsRequired();

                // Map đúng cặp navigation để tránh shadow property UserId1
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)   // User.Notifications
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // Feedback
            // =========================
            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Content).IsRequired();
                entity.Property(f => f.Status).HasMaxLength(50);
                entity.Property(f => f.CreatedAt).IsRequired();

                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)       // nếu chưa có property, có thể đổi thành .WithMany()
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // Payment
            // =========================
            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(p => p.PaymentDate)
                      .IsRequired();
                entity.Property(p => p.PaymentMethod)
                      .IsRequired();
                entity.Property(p => p.Status)
                      .HasMaxLength(50);

                // Payment -> User (N-1)
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Payments)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
