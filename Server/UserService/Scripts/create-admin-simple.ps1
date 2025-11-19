# Simple script to create admin user for TSL Admin Panel
# Usage: .\create-admin-simple.ps1

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Creating Admin User for TSL Admin Panel" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to Scripts directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$createAdminPath = Join-Path $scriptPath "CreateAdminUser"
Set-Location $createAdminPath

# Check if dotnet is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: .NET SDK is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Check if project exists
if (-not (Test-Path "CreateAdminUser.csproj")) {
    Write-Host "Error: CreateAdminUser.csproj not found" -ForegroundColor Red
    exit 1
}

# Build and run
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build --no-restore 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Restoring packages and rebuilding..." -ForegroundColor Yellow
    dotnet restore
    dotnet build
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "Running script..." -ForegroundColor Yellow
    Write-Host ""
    dotnet run
} else {
    Write-Host "Build failed. Please check the error messages above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Script completed!" -ForegroundColor Green

