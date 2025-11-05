# Quick API Gateway Test
Write-Host "Testing API Gateway..." -ForegroundColor Cyan

# Wait a bit for the gateway to start
Start-Sleep -Seconds 3

# Test Health Endpoint
try {
    Write-Host "`n[1] Testing Health Endpoint..." -ForegroundColor Yellow
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/health" -Method GET -ErrorAction Stop
    Write-Host "✓ Health Check: PASSED" -ForegroundColor Green
    Write-Host "  Status: $($response.status)" -ForegroundColor Gray
    Write-Host "  Timestamp: $($response.timestamp)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Health Check: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Make sure API Gateway is running on port 5000" -ForegroundColor Yellow
}

# Test Swagger
try {
    Write-Host "`n[2] Testing Swagger UI..." -ForegroundColor Yellow
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method GET -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Swagger UI: PASSED" -ForegroundColor Green
    }
} catch {
    Write-Host "✗ Swagger UI: FAILED" -ForegroundColor Red
}

# Test Request ID
try {
    Write-Host "`n[3] Testing Request ID..." -ForegroundColor Yellow
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -Method GET -UseBasicParsing -ErrorAction Stop
    if ($response.Headers["X-Request-Id"]) {
        Write-Host "✓ Request ID: PASSED" -ForegroundColor Green
        Write-Host "  Request ID: $($response.Headers['X-Request-Id'])" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ Request ID: FAILED" -ForegroundColor Red
}

Write-Host "`nTest completed!" -ForegroundColor Cyan
Write-Host "`nTo start API Gateway, run: dotnet run" -ForegroundColor Yellow
Write-Host "Then open: http://localhost:5000/swagger" -ForegroundColor Yellow

