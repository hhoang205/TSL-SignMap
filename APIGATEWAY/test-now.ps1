# Quick API Gateway Test
Write-Host "`n=== API Gateway Test ===" -ForegroundColor Cyan
Write-Host ""

# Wait a bit
Start-Sleep -Seconds 2

# Test 1: Health Check
Write-Host "[1] Testing Health Endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/health" -Method GET -ErrorAction Stop
    Write-Host "  ✓ Health Check: PASSED" -ForegroundColor Green
    Write-Host "  Status: $($response.status)" -ForegroundColor Gray
    Write-Host "  Timestamp: $($response.timestamp)" -ForegroundColor Gray
} catch {
    Write-Host "  ✗ Health Check: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Make sure API Gateway is running: dotnet run" -ForegroundColor Yellow
    exit
}

# Test 2: Swagger UI
Write-Host "`n[2] Testing Swagger UI..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method GET -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "  ✓ Swagger UI: PASSED" -ForegroundColor Green
        Write-Host "  Open: http://localhost:5000/swagger" -ForegroundColor Cyan
    }
} catch {
    Write-Host "  ✗ Swagger UI: FAILED" -ForegroundColor Red
}

# Test 3: Request ID
Write-Host "`n[3] Testing Request ID Middleware..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -Method GET -UseBasicParsing -ErrorAction Stop
    if ($response.Headers["X-Request-Id"]) {
        Write-Host "  ✓ Request ID: PASSED" -ForegroundColor Green
        Write-Host "  Request ID: $($response.Headers['X-Request-Id'])" -ForegroundColor Gray
    } else {
        Write-Host "  ⚠ Request ID header not found" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ✗ Request ID: FAILED" -ForegroundColor Red
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Cyan
Write-Host ""

