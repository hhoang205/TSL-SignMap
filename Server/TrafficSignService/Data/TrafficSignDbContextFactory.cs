using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TrafficSignService.Data
{
    /// Factory class for creating TrafficSignDbContext instances at design time.
    /// This is required for EF Core migrations when running in Docker or without a running application.
    public class TrafficSignDbContextFactory : IDesignTimeDbContextFactory<TrafficSignDbContext>
    {
        public TrafficSignDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TrafficSignDbContext>();
            
            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // If connection string is not found, use a default for design-time
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=localhost,1433;Database=TFSIGN;User ID=sa;Password=Admin123@;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30;MultipleActiveResultSets=True;ApplicationIntent=ReadWrite";
            }

            optionsBuilder.UseSqlServer(
                connectionString,
                x => x.UseNetTopologySuite());

            return new TrafficSignDbContext(optionsBuilder.Options);
        }
    }
}

