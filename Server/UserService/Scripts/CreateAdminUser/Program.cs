using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.Data;
using UserService.Models;

// Simple console app to create admin user
// Get the UserService directory (parent of Scripts)
var userServicePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."));

var configuration = new ConfigurationBuilder()
    .SetBasePath(userServicePath)
    .AddJsonFile(Path.Combine(userServicePath, "appsettings.json"), optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Server=localhost,1433;Database=TFSIGN;User ID=sa;Password=Admin123@;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30;MultipleActiveResultSets=True;ApplicationIntent=ReadWrite";
}

var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
optionsBuilder.UseSqlServer(connectionString, x => x.UseNetTopologySuite());

using var context = new UserDbContext(optionsBuilder.Options);
var passwordHasher = new PasswordHasher<User>();

// Admin credentials
var adminEmail = "admin@.com";
var adminUsername = "admin02";
var adminPassword = "Admin123@";
var adminPhone = "+84123456789";

Console.WriteLine("Checking for existing admin user...");

// Check if admin already exists
var existingAdmin = await context.Users
    .FirstOrDefaultAsync(u => u.Email == adminEmail || u.Username == adminUsername);

if (existingAdmin != null)
{
    Console.WriteLine($"Admin user already exists with ID: {existingAdmin.Id}");
    Console.WriteLine($"Email: {existingAdmin.Email}, Username: {existingAdmin.Username}, RoleId: {existingAdmin.RoleId}");
    
    // Update to admin role if not already
    if (existingAdmin.RoleId != 2)
    {
        existingAdmin.RoleId = 2;
        existingAdmin.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        Console.WriteLine("Updated user role to Admin (RoleId = 2)");
    }
    
    // Check if wallet exists
    var wallet = await context.CoinWallets.FirstOrDefaultAsync(w => w.UserId == existingAdmin.Id);
    if (wallet == null)
    {
        wallet = new CoinWallet
        {
            Id = Guid.NewGuid(),
            UserId = existingAdmin.Id,
            Balance = 1000m,
            CreatedAt = DateTime.UtcNow
        };
        await context.CoinWallets.AddAsync(wallet);
        await context.SaveChangesAsync();
        Console.WriteLine("Created wallet for admin with 1000 coins");
    }
    
    Console.WriteLine("\n=========================================");
    Console.WriteLine("Admin credentials:");
    Console.WriteLine($"Email: {adminEmail}");
    Console.WriteLine($"Password: {adminPassword}");
    Console.WriteLine("=========================================");
    return;
}

Console.WriteLine("Creating new admin user...");

// Create new admin user
var adminUser = new User
{
    Username = adminUsername,
    Email = adminEmail,
    PhoneNumber = adminPhone,
    Firstname = "Admin",
    Lastname = "User",
    RoleId = 2, // 2: admin
    Reputation = 0f,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
};

adminUser.Password = passwordHasher.HashPassword(adminUser, adminPassword);

await context.Users.AddAsync(adminUser);
await context.SaveChangesAsync();

// Create wallet for admin
var adminWallet = new CoinWallet
{
    Id = Guid.NewGuid(),
    UserId = adminUser.Id,
    Balance = 1000m, // Admin gets more coins
    CreatedAt = DateTime.UtcNow
};

await context.CoinWallets.AddAsync(adminWallet);
await context.SaveChangesAsync();

Console.WriteLine("\n=========================================");
Console.WriteLine("Admin user created successfully!");
Console.WriteLine("=========================================");
Console.WriteLine($"User ID: {adminUser.Id}");
Console.WriteLine($"Username: {adminUser.Username}");
Console.WriteLine($"Email: {adminUser.Email}");
Console.WriteLine($"Password: {adminPassword}");
Console.WriteLine($"RoleId: {adminUser.RoleId} (Admin)");
Console.WriteLine($"Wallet Balance: {adminWallet.Balance} coins");
Console.WriteLine("=========================================");
Console.WriteLine("\nYou can now login to the admin panel with:");
Console.WriteLine($"Email: {adminEmail}");
Console.WriteLine($"Password: {adminPassword}");
Console.WriteLine("=========================================");

