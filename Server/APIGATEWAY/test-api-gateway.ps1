# API Gateway Test Script
# Test các endpoints của API Gateway

$baseUrl = "http://localhost:5000"
$headers = @{
    "Content-Type" = "application/json"
}

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "API Gateway Test Suite" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Health Check
Write-Host "Test 1: Health Check Endpoint" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/health" -Method GET -Headers $headers
    Write-Host "[PASS] Health Check: PASSED" -ForegroundColor Green
    Write-Host "  Status: $($response.status)" -ForegroundColor Gray
    Write-Host "  Timestamp: $($response.timestamp)" -ForegroundColor Gray
    Write-Host "  Services:" -ForegroundColor Gray
    $response.services | ForEach-Object {
        Write-Host "    - $($_.name): $($_.status)" -ForegroundColor Gray
    }
} catch {
    Write-Host "[FAIL] Health Check: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Swagger UI
Write-Host "Test 2: Swagger UI Endpoint" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/swagger" -Method GET -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        $status = $response.StatusCode
        Write-Host "[PASS] Swagger UI: PASSED (Status: $status)" -ForegroundColor Green
    } else {
        $status = $response.StatusCode
        Write-Host "[FAIL] Swagger UI: FAILED (Status: $status)" -ForegroundColor Red
    }
} catch {
    Write-Host "[FAIL] Swagger UI: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: CORS Preflight
Write-Host "Test 3: CORS Preflight Request" -ForegroundColor Yellow
try {
    $corsHeaders = @{
        "Origin" = "http://localhost:3000"
        "Access-Control-Request-Method" = "GET"
        "Access-Control-Request-Headers" = "Authorization"
    }
    $response = Invoke-WebRequest -Uri "$baseUrl/api/health" -Method OPTIONS -Headers $corsHeaders -UseBasicParsing
    if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 204) {
        $status = $response.StatusCode
        Write-Host "[PASS] CORS Preflight: PASSED (Status: $status)" -ForegroundColor Green
        $accessControl = $response.Headers["Access-Control-Allow-Origin"]
        if ($accessControl) {
            Write-Host "  Access-Control-Allow-Origin: $accessControl" -ForegroundColor Gray
        }
    } else {
        $status = $response.StatusCode
        Write-Host "[FAIL] CORS Preflight: FAILED (Status: $status)" -ForegroundColor Red
    }
} catch {
    Write-Host "[FAIL] CORS Preflight: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Rate Limiting (Test với nhiều requests)
Write-Host "Test 4: Rate Limiting (Sending 5 requests)" -ForegroundColor Yellow
$rateLimitPassed = $true
for ($i = 1; $i -le 5; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/api/health" -Method GET -UseBasicParsing
        $status = $response.StatusCode
        if ($status -eq 200) {
            Write-Host "  Request ${i}: OK (Status: $status)" -ForegroundColor Gray
        } else {
            Write-Host "  Request ${i}: Unexpected Status $status" -ForegroundColor Yellow
        }
        Start-Sleep -Milliseconds 200
    } catch {
        $exStatus = $_.Exception.Response.StatusCode.value__
        if ($exStatus -eq 429) {
            Write-Host "  Request ${i}: Rate Limited (429)" -ForegroundColor Yellow
        } else {
            Write-Host "  Request ${i}: Error - $($_.Exception.Message)" -ForegroundColor Red
            $rateLimitPassed = $false
        }
    }
}
if ($rateLimitPassed) {
    Write-Host "[PASS] Rate Limiting Test: PASSED" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Rate Limiting Test: FAILED" -ForegroundColor Red
}
Write-Host ""

# Test 5: Security Headers
Write-Host "Test 5: Security Headers" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/health" -Method GET -UseBasicParsing
    $headersToCheck = @(
        "X-Content-Type-Options",
        "X-Frame-Options",
        "X-XSS-Protection",
        "Referrer-Policy",
        "Content-Security-Policy"
    )
    $foundHeaders = 0
    foreach ($headerName in $headersToCheck) {
        if ($response.Headers[$headerName]) {
            $headerValue = $response.Headers[$headerName]
            Write-Host "  [OK] $headerName : $headerValue" -ForegroundColor Gray
            $foundHeaders++
        }
    }
    if ($foundHeaders -gt 0) {
        Write-Host "[PASS] Security Headers: PASSED ($foundHeaders headers found)" -ForegroundColor Green
    } else {
        Write-Host "[WARN] Security Headers: No headers found (may be normal)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "[FAIL] Security Headers: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 6: Request ID Middleware
Write-Host "Test 6: Request ID Middleware" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/health" -Method GET -UseBasicParsing
    if ($response.Headers["X-Request-Id"]) {
        Write-Host "[PASS] Request ID: PASSED" -ForegroundColor Green
        Write-Host "  Request ID: $($response.Headers['X-Request-Id'])" -ForegroundColor Gray
    } else {
        Write-Host "[WARN] Request ID: Not found in headers" -ForegroundColor Yellow
    }
} catch {
    Write-Host "[FAIL] Request ID: FAILED" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 7: Error Handling (Test invalid endpoint)
Write-Host "Test 7: Error Handling (Invalid Endpoint)" -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/invalid-endpoint" -Method GET -UseBasicParsing -ErrorAction Stop
    $status = $response.StatusCode
    Write-Host "[WARN] Error Handling: Got response (Status: $status)" -ForegroundColor Yellow
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 404) {
        Write-Host "[PASS] Error Handling: PASSED (404 Not Found)" -ForegroundColor Green
    } else {
        Write-Host "[WARN] Error Handling: Got status $statusCode" -ForegroundColor Yellow
    }
}
Write-Host ""

# Test 8: Proxy Routes (Test nếu backend services đang chạy)
Write-Host "Test 8: Proxy Routes (Backend services may not be running)" -ForegroundColor Yellow
$routes = @(
    "/api/users",
    "/api/signs",
    "/api/contributions"
)

foreach ($route in $routes) {
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl$route" -Method GET -UseBasicParsing -ErrorAction Stop -TimeoutSec 5
        $status = $response.StatusCode
        Write-Host "  $route : Status $status" -ForegroundColor Gray
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 401) {
            Write-Host "  $route : 401 Unauthorized (Authentication required)" -ForegroundColor Yellow
        } elseif ($statusCode -eq 502 -or $statusCode -eq 503) {
            Write-Host "  $route : $statusCode (Backend service not available)" -ForegroundColor Yellow
        } elseif ($statusCode -eq 404) {
            Write-Host "  $route : 404 Not Found" -ForegroundColor Yellow
        } else {
            Write-Host "  $route : Error - $($_.Exception.Message)" -ForegroundColor Gray
        }
    }
}
Write-Host ""

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Test Suite Completed" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Note: Some tests may fail if backend services are not running." -ForegroundColor Gray
Write-Host "This is expected behavior for proxy routes." -ForegroundColor Gray

