#!/bin/bash
# API Gateway Test Script (Bash version for Linux/Mac)

BASE_URL="http://localhost:5000"

echo "=========================================="
echo "API Gateway Test Suite"
echo "=========================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
GRAY='\033[0;37m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Test 1: Health Check
echo -e "${YELLOW}Test 1: Health Check Endpoint${NC}"
response=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/health")
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if [ "$http_code" -eq 200 ]; then
    echo -e "${GREEN}✓ Health Check: PASSED${NC}"
    echo -e "${GRAY}  Status Code: $http_code${NC}"
    echo "$body" | jq '.' 2>/dev/null || echo "$body"
else
    echo -e "${RED}✗ Health Check: FAILED (Status: $http_code)${NC}"
fi
echo ""

# Test 2: Swagger UI
echo -e "${YELLOW}Test 2: Swagger UI Endpoint${NC}"
http_code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/swagger")
if [ "$http_code" -eq 200 ]; then
    echo -e "${GREEN}✓ Swagger UI: PASSED (Status: $http_code)${NC}"
else
    echo -e "${RED}✗ Swagger UI: FAILED (Status: $http_code)${NC}"
fi
echo ""

# Test 3: CORS Preflight
echo -e "${YELLOW}Test 3: CORS Preflight Request${NC}"
http_code=$(curl -s -o /dev/null -w "%{http_code}" \
    -X OPTIONS \
    -H "Origin: http://localhost:3000" \
    -H "Access-Control-Request-Method: GET" \
    -H "Access-Control-Request-Headers: Authorization" \
    "$BASE_URL/api/health")

if [ "$http_code" -eq 200 ] || [ "$http_code" -eq 204 ]; then
    echo -e "${GREEN}✓ CORS Preflight: PASSED (Status: $http_code)${NC}"
    access_control=$(curl -s -I "$BASE_URL/api/health" | grep -i "access-control-allow-origin" || echo "")
    if [ -n "$access_control" ]; then
        echo -e "${GRAY}  $access_control${NC}"
    fi
else
    echo -e "${RED}✗ CORS Preflight: FAILED (Status: $http_code)${NC}"
fi
echo ""

# Test 4: Rate Limiting
echo -e "${YELLOW}Test 4: Rate Limiting (Sending 5 requests)${NC}"
rate_limit_passed=true
for i in {1..5}; do
    http_code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/api/health")
    if [ "$http_code" -eq 200 ]; then
        echo -e "${GRAY}  Request $i: OK (Status: $http_code)${NC}"
    elif [ "$http_code" -eq 429 ]; then
        echo -e "${YELLOW}  Request $i: Rate Limited (429)${NC}"
    else
        echo -e "${RED}  Request $i: Error (Status: $http_code)${NC}"
        rate_limit_passed=false
    fi
    sleep 0.2
done

if [ "$rate_limit_passed" = true ]; then
    echo -e "${GREEN}✓ Rate Limiting Test: PASSED${NC}"
else
    echo -e "${RED}✗ Rate Limiting Test: FAILED${NC}"
fi
echo ""

# Test 5: Security Headers
echo -e "${YELLOW}Test 5: Security Headers${NC}"
headers=$(curl -s -I "$BASE_URL/api/health")
found_headers=0

if echo "$headers" | grep -qi "X-Content-Type-Options"; then
    echo -e "${GRAY}  ✓ X-Content-Type-Options found${NC}"
    ((found_headers++))
fi
if echo "$headers" | grep -qi "X-Frame-Options"; then
    echo -e "${GRAY}  ✓ X-Frame-Options found${NC}"
    ((found_headers++))
fi
if echo "$headers" | grep -qi "X-XSS-Protection"; then
    echo -e "${GRAY}  ✓ X-XSS-Protection found${NC}"
    ((found_headers++))
fi

if [ "$found_headers" -gt 0 ]; then
    echo -e "${GREEN}✓ Security Headers: PASSED ($found_headers headers found)${NC}"
else
    echo -e "${YELLOW}⚠ Security Headers: No headers found (may be normal)${NC}"
fi
echo ""

# Test 6: Request ID Middleware
echo -e "${YELLOW}Test 6: Request ID Middleware${NC}"
request_id=$(curl -s -I "$BASE_URL/api/health" | grep -i "X-Request-Id" || echo "")
if [ -n "$request_id" ]; then
    echo -e "${GREEN}✓ Request ID: PASSED${NC}"
    echo -e "${GRAY}  $request_id${NC}"
else
    echo -e "${YELLOW}⚠ Request ID: Not found in headers${NC}"
fi
echo ""

# Test 7: Error Handling
echo -e "${YELLOW}Test 7: Error Handling (Invalid Endpoint)${NC}"
http_code=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/api/invalid-endpoint")
if [ "$http_code" -eq 404 ]; then
    echo -e "${GREEN}✓ Error Handling: PASSED (404 Not Found)${NC}"
else
    echo -e "${YELLOW}⚠ Error Handling: Got status $http_code${NC}"
fi
echo ""

# Test 8: Proxy Routes
echo -e "${YELLOW}Test 8: Proxy Routes (Backend services may not be running)${NC}"
routes=("/api/users" "/api/signs" "/api/contributions")

for route in "${routes[@]}"; do
    http_code=$(curl -s -o /dev/null -w "%{http_code}" -m 5 "$BASE_URL$route")
    if [ "$http_code" -eq 401 ]; then
        echo -e "${YELLOW}  $route : 401 Unauthorized (Authentication required)${NC}"
    elif [ "$http_code" -eq 502 ] || [ "$http_code" -eq 503 ]; then
        echo -e "${YELLOW}  $route : $http_code (Backend service not available)${NC}"
    elif [ "$http_code" -eq 404 ]; then
        echo -e "${YELLOW}  $route : 404 Not Found${NC}"
    else
        echo -e "${GRAY}  $route : Status $http_code${NC}"
    fi
done
echo ""

echo "=========================================="
echo -e "${CYAN}Test Suite Completed${NC}"
echo "=========================================="
echo ""
echo -e "${GRAY}Note: Some tests may fail if backend services are not running.${NC}"
echo -e "${GRAY}This is expected behavior for proxy routes.${NC}"

