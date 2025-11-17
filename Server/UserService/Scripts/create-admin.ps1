# Script to create admin user for TSL Admin Panel
# This script creates an admin user in the database

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Creating Admin User for TSL Admin Panel" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to UserService directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$userServicePath = Join-Path $scriptPath ".."
Set-Location $userServicePath

# Check if dotnet is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: .NET SDK is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Check if appsettings.json exists
if (-not (Test-Path "appsettings.json")) {
    Write-Host "Error: appsettings.json not found in UserService directory" -ForegroundColor Red
    exit 1
}

# Create a temporary C# program file
$tempScript = @"
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.Data;
using UserService.Models;

// Simple script to create admin user
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
var adminEmail = "admin@tsl.com";
var adminUsername = "admin";
var adminPassword = "Admin123@";
var adminPhone = "+84123456789";

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
    
    return;
}

// Create new admin user
var adminUser = new User
{
    Username = adminUsername,
    Email = adminEmail,
    PhoneNumber = adminPhone,
    Firstname = "Admin",
    Lastname = "User",
    RoleId = 2,
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
    Balance = 1000m,
    CreatedAt = DateTime.UtcNow
};

await context.CoinWallets.AddAsync(adminWallet);
await context.SaveChangesAsync();

Console.WriteLine("=========================================");
Console.WriteLine("Admin user created successfully!");
Console.WriteLine("=========================================");
Console.WriteLine($"User ID: {adminUser.Id}");
Console.WriteLine($"Username: {adminUser.Username}");
Console.WriteLine($"Email: {adminUser.Email}");
Console.WriteLine($"Password: {adminPassword}");
Console.WriteLine($"RoleId: {adminUser.RoleId} (Admin)");
Console.WriteLine($"Wallet Balance: {adminWallet.Balance} coins");
Console.WriteLine("=========================================");
Console.WriteLine("You can now login to the admin panel with:");
Console.WriteLine($"Email: {adminEmail}");
Console.WriteLine($"Password: {adminPassword}");
Console.WriteLine("=========================================");
"@

# Write temporary script
$tempScriptPath = Join-Path $userServicePath "CreateAdminUser.cs"
$tempScript | Out-File -FilePath $tempScriptPath -Encoding UTF8

# Run the script using dotnet-script if available, otherwise use a different approach
if (Get-Command dotnet-script -ErrorAction SilentlyContinue) {
    Write-Host "Running script with dotnet-script..." -ForegroundColor Yellow
    dotnet-script $tempScriptPath
} else {
    # Alternative: Create a simple console app
    Write-Host "dotnet-script not found. Creating temporary console app..." -ForegroundColor Yellow
    
    $tempProjectPath = Join-Path $userServicePath "CreateAdminUser"
    New-Item -ItemType Directory -Path $tempProjectPath -Force | Out-Null
    
    # Create project file
    $projectFile = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\UserService.csproj" />
  </ItemGroup>
</Project>
"@
    $projectFile | Out-File -FilePath (Join-Path $tempProjectPath "CreateAdminUser.csproj") -Encoding UTF8
    
    # Copy script to Program.cs
    Copy-Item $tempScriptPath (Join-Path $tempProjectPath "Program.cs")
    
    # Build and run
    Set-Location $tempProjectPath
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        dotnet run
    } else {
        Write-Host "Build failed. Trying alternative method..." -ForegroundColor Yellow
        # Clean up
        Remove-Item -Path $tempProjectPath -Recurse -Force
        Remove-Item -Path $tempScriptPath -Force
        Write-Host "Please run the UserService and use the API endpoint to create admin user, or use SQL script." -ForegroundColor Yellow
        exit 1
    }
    
    # Clean up
    Set-Location $userServicePath
    Remove-Item -Path $tempProjectPath -Recurse -Force
}

# Clean up temp script
Remove-Item -Path $tempScriptPath -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "Script completed!" -ForegroundColor Green

