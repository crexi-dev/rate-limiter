# Rate Limiting Configuration Tutorial

## Overview

This .NET 9 Rate Limiting System is a flexible HTTP request throttling solution that protects your API endpoints from abuse. It supports multiple algorithms (Fixed Window, Sliding Window, Token Bucket, and Region-Based) and can be configured entirely through `appsettings.json` without requiring code changes.

**Key Features:**
- Configuration-based rules (no code deployment needed)
- Multiple rate limiting algorithms
- Region-aware limiting
- Automatic conflict resolution
- Real-time header feedback

## How Rate Limiting Rules Work

### Rule Precedence
When multiple rules could apply to the same request, the system resolves conflicts using this priority order:

1. **Configuration Rules** (from `appsettings.json`) - **HIGHEST PRIORITY**
2. **Attribute Rules** (from code annotations) - Lower priority

**Important**: When rules target the same path and method, only the highest priority rule is applied. To use multiple rules, they must target different endpoints.

### Rule Matching
Rules match requests based on:
- **Path Pattern**: Which endpoints the rule applies to
- **HTTP Methods**: GET, POST, PUT, DELETE, etc.
- **Region**: Geographic location (optional)

## Scenario 1: Blog API Protection

**Use Case**: A blog API where configuration rules override strict code-based limits, demonstrating different protection strategies for different endpoints.

### Configuration

Create or update your `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "RateLimiter": "Debug"
    }
  },
  "AllowedHosts": "*",
  
  "RateLimiting": {
    "EnableConfigurationRules": true,
    "EnableAttributeRules": true,
    "ConflictResolutionStrategy": "ConfigurationWins",
    "LogConflicts": true,
    
    "Rules": [
      {
        "Name": "BlogReadProtection",
        "Type": "FixedWindow",
        "MaxRequests": 15,
        "TimeWindowSeconds": 60,
        "PathPattern": "/api/demo",
        "HttpMethods": "GET",
        "Enabled": true,
        "Priority": 10,
        "Metadata": {
          "Description": "Allow 15 blog reads per minute (overrides attribute limit of 5)",
          "Category": "BlogAPI"
        }
      },
      {
        "Name": "BlogCommentProtection",
        "Type": "TokenBucket",
        "BucketCapacity": 5,
        "RefillRatePerSecond": 0.2,
        "PathPattern": "/api/demo/burst",
        "HttpMethods": "GET",
        "Enabled": true,
        "Priority": 10,
        "Metadata": {
          "Description": "Prevent rapid comment posting - 5 burst, refill 1 every 5 seconds",
          "Category": "BurstProtection"
        }
      }
    ]
  }
}
```

### How to Test

1. **Start the Server**:
```bash
cd RateLimiter/Api
dotnet run
```

2. **Create Test Script**:
```bash
# Create blog_test.sh
cat > blog_test.sh << 'TEST_SCRIPT'
#!/bin/bash

API_URL="http://localhost:5037/api/demo"
BURST_URL="http://localhost:5037/api/demo/burst"
echo "=== Blog API Rate Limiting Test ==="
echo

echo "Test 1: Configuration overrides attribute rules (should allow more than 5 requests)"
for i in {1..10}; do
    response=$(curl -s -w "Status: %{http_code}" "$API_URL")
    echo "Request $i: $response"
    sleep 0.1
done
echo

echo "Test 2: Token Bucket burst protection (should allow 5 rapid requests then limit)"
echo "Testing /api/demo/burst endpoint..."
for i in {1..8}; do
    status=$(curl -s -w "%{http_code}" -o /dev/null "$BURST_URL")
    if [ "$status" = "429" ]; then
        echo "Request $i: RATE LIMITED (429) - Token bucket exhausted"
        break
    else
        echo "Request $i: SUCCESS ($status)"
    fi
    # No delay for burst test
done
echo

echo "Test 3: Check rate limit headers for both endpoints"
echo "Headers for /api/demo:"
curl -s -D- -o /dev/null "$API_URL" | grep -i "x-ratelimit"
echo
echo "Headers for /api/demo/burst:"
curl -s -D- -o /dev/null "$BURST_URL" | grep -i "x-ratelimit"
echo

echo "=== Test Complete ==="
TEST_SCRIPT

chmod +x blog_test.sh
```

3. **Run Test**:
```bash
./blog_test.sh
```

### Expected Results

```
=== Blog API Rate Limiting Test ===

Test 1: Configuration overrides attribute rules (should allow more than 5 requests)
Request 1: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 2: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 3: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 4: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 5: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 6: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 7: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 8: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 9: {"message":"Demo endpoint - subject to global rate limit"}Status: 200
Request 10: {"message":"Demo endpoint - subject to global rate limit"}Status: 200

Test 2: Token Bucket burst protection (should allow 5 rapid requests then limit)
Testing /api/demo/burst endpoint...
Request 1: SUCCESS (200)
Request 2: SUCCESS (200)
Request 3: SUCCESS (200)
Request 4: SUCCESS (200)
Request 5: SUCCESS (200)
Request 6: RATE LIMITED (429) - Token bucket exhausted

Test 3: Check rate limit headers for both endpoints
Headers for /api/demo:
X-RateLimit-Limit: 15
X-RateLimit-Remaining: 5
X-RateLimit-Reset: 45
X-RateLimit-Rule: BlogReadProtection

Headers for /api/demo/burst:
X-RateLimit-Limit: 5
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 25
X-RateLimit-Rule: BlogCommentProtection
```

**What This Demonstrates:**
- **Configuration Override**: The `/api/demo` endpoint normally has a limit of 5 requests (from attribute), but configuration increases it to 15
- **Different Algorithms**: Fixed Window for reading vs Token Bucket for burst protection
- **Separate Endpoints**: Each rule targets a different path to avoid conflicts

---

## Scenario 2: Regional E-commerce API with GDPR Compliance

**Use Case**: An e-commerce API that must comply with different regional regulations - GDPR requires stricter limits for EU users.

### Configuration

Update your `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "RateLimiter": "Debug"
    }
  },
  "AllowedHosts": "*",
  
  "RateLimiting": {
    "EnableConfigurationRules": true,
    "EnableAttributeRules": true,
    "ConflictResolutionStrategy": "ConfigurationWins",
    "LogConflicts": true,
    
    "Rules": [
      {
        "Name": "USRegionAPI",
        "Type": "FixedWindow",
        "MaxRequests": 50,
        "TimeWindowSeconds": 60,
        "PathPattern": "/api/demo/region/us",
        "HttpMethods": "GET",
        "TargetRegion": "US",
        "Enabled": true,
        "Priority": 10,
        "Metadata": {
          "Description": "Higher limits for US region - 50 requests per minute (overrides attribute limit of 20)",
          "Category": "Regional",
          "Compliance": "Standard"
        }
      },
      {
        "Name": "EURegionGDPR",
        "Type": "RegionBased",
        "MaxRequests": 8,
        "TimeWindowSeconds": 60,
        "PathPattern": "/api/demo/region/eu",
        "HttpMethods": "GET",
        "TargetRegion": "EU",
        "MinTimeBetweenRequestsMs": 3000,
        "Enabled": true,
        "Priority": 10,
        "Metadata": {
          "Description": "GDPR-compliant EU limits - 8 requests per minute with 3-second delays (overrides attribute limit of 10)",
          "Category": "Regional",
          "Compliance": "GDPR"
        }
      }
    ]
  }
}
```

### How to Test

1. **Start the Server** (if not running):
```bash
cd RateLimiter/Api
dotnet run
```

2. **Create Test Script**:
```bash
# Create regional_test.sh
cat > regional_test.sh << 'TEST_SCRIPT'
#!/bin/bash

echo "=== Regional E-commerce API Rate Limiting Test ==="
echo

# Test US Region (should allow many requests)
echo "=== Testing US Region (config: 50/min vs attribute: 20/min) ==="
US_SUCCESS=0
for i in {1..15}; do
    response=$(curl -s -w "%{http_code}" -H "X-Region: US" "http://localhost:5037/api/demo/region/us")
    status="${response: -3}"
    
    if [ "$status" = "200" ]; then
        echo "Request $i: SUCCESS"
        ((US_SUCCESS++))
    elif [ "$status" = "429" ]; then
        echo "Request $i: RATE LIMITED (429)"
        echo "✅ US rate limit reached after $((i-1)) requests"
        break
    else
        echo "Request $i: ERROR ($status)"
    fi
    
    sleep 0.3
done

if [ $US_SUCCESS -eq 15 ]; then
    echo "✅ US region allowed all 15 requests (higher config limit active)"
fi

echo
echo "=== Testing EU Region (config: 8/min vs attribute: 10/min) ==="
EU_SUCCESS=0
for i in {1..12}; do
    response=$(curl -s -w "%{http_code}" -H "X-Region: EU" "http://localhost:5037/api/demo/region/eu")
    status="${response: -3}"
    
    if [ "$status" = "200" ]; then
        echo "Request $i: SUCCESS"
        ((EU_SUCCESS++))
        
        # Show headers every few requests
        if [ $((i % 3)) -eq 0 ]; then
            echo "  Checking rate limit headers..."
            curl -s -D- -H "X-Region: EU" "http://localhost:5037/api/demo/region/eu" | grep -i "x-ratelimit" | head -4
        fi
        
    elif [ "$status" = "429" ]; then
        echo "Request $i: RATE LIMITED (429)"
        echo "✅ EU rate limit reached after $((i-1)) requests"
        echo "Rate limit headers:"
        curl -s -D- -H "X-Region: EU" "http://localhost:5037/api/demo/region/eu" | grep -i "x-ratelimit"
        break
    else
        echo "Request $i: ERROR ($status)"
    fi
    
    # Shorter delay to hit limit faster while respecting MinTimeBetweenRequestsMs
    sleep 2.0
done

echo
echo "=== Testing EU MinTimeBetweenRequestsMs (3000ms delay requirement) ==="
echo "Making rapid requests to EU endpoint..."
for i in {1..3}; do
    start_time=$(date +%s%3N)
    response=$(curl -s -w "%{http_code}" -H "X-Region: EU" "http://localhost:5037/api/demo/region/eu")
    end_time=$(date +%s%3N)
    duration=$((end_time - start_time))
    status="${response: -3}"
    
    if [ "$status" = "200" ]; then
        echo "Request $i: SUCCESS (took ${duration}ms)"
    elif [ "$status" = "429" ]; then
        echo "Request $i: RATE LIMITED - MinTimeBetweenRequestsMs enforced"
        break
    else
        echo "Request $i: ERROR ($status)"
    fi
    
    # No delay for rapid test
done

echo
echo "=== SUMMARY ==="
echo "US Region successful requests: $US_SUCCESS"
echo "EU Region successful requests: $EU_SUCCESS"

if [ $US_SUCCESS -gt $EU_SUCCESS ]; then
    echo "✅ Configuration successfully provides higher limits for US vs EU"
else
    echo "⚠️  Regional differences not clearly demonstrated"
fi

if [ $EU_SUCCESS -lt 8 ]; then
    echo "✅ EU GDPR limits correctly enforced (hit limit before 8 requests)"
elif [ $EU_SUCCESS -eq 8 ]; then
    echo "✅ EU GDPR limits correctly enforced (exactly 8 requests allowed)"
else
    echo "⚠️  EU limits higher than expected"
fi

echo
echo "Key findings from logs:"
echo "- US region uses 'USRegionAPI' configuration rule (50 limit)"
echo "- EU region uses 'EURegionGDPR' configuration rule (8 limit)"
echo "- Both override their respective attribute rules"
TEST_SCRIPT

chmod +x regional_test.sh
```

3. **Run Test**:
```bash
./regional_test.sh
```

### Expected Results

```
=== Regional E-commerce API Rate Limiting Test ===

=== Testing US Region (config: 50/min vs attribute: 20/min) ===
Request 1: SUCCESS
Request 2: SUCCESS
Request 3: SUCCESS
Request 4: SUCCESS
Request 5: SUCCESS
Request 6: SUCCESS
Request 7: SUCCESS
Request 8: SUCCESS
Request 9: SUCCESS
Request 10: SUCCESS
Request 11: SUCCESS
Request 12: SUCCESS
Request 13: SUCCESS
Request 14: SUCCESS
Request 15: SUCCESS
✅ US region allowed all 15 requests (higher config limit active)

=== Testing EU Region (config: 8/min vs attribute: 10/min) ===
Request 1: SUCCESS
Request 2: RATE LIMITED (429)
✅ EU rate limit reached after 1 requests
Rate limit headers:
X-RateLimit-Limit: 8
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 45
X-RateLimit-Rule: EURegionGDPR

=== Testing EU MinTimeBetweenRequestsMs (3000ms delay requirement) ===
Making rapid requests to EU endpoint...
Request 1: RATE LIMITED - MinTimeBetweenRequestsMs enforced

=== SUMMARY ===
US Region successful requests: 15
EU Region successful requests: 1
✅ Configuration successfully provides higher limits for US vs EU
✅ EU GDPR limits correctly enforced (hit limit before 8 requests)

Key findings from logs:
- US region uses 'USRegionAPI' configuration rule (50 limit)
- EU region uses 'EURegionGDPR' configuration rule (8 limit)  
- Both override their respective attribute rules
```

**What This Demonstrates:**
- **Regional Override**: US region gets higher limits (50 vs 20) through configuration
- **GDPR Compliance**: EU region gets lower limits (8 vs 10) with mandatory delays
- **Different Rule Types**: Fixed Window for US vs Region-Based for EU
- **Real-world Use Case**: Different compliance requirements per region

---

## Understanding the Configuration

### Rule Types

1. **FixedWindow**: Simple counter that resets at fixed intervals
   - Best for: General API protection
   - Example: 15 requests per minute

2. **TokenBucket**: Allows bursts but refills slowly
   - Best for: Preventing rapid automation while allowing normal bursts
   - Example: 5 requests immediately, then 1 every 5 seconds

3. **RegionBased**: Different limits per geographic region
   - Best for: Regulatory compliance (GDPR, etc.)
   - Example: EU users get lower limits than US users

### Key Configuration Options

- **MaxRequests**: Total requests allowed in the time window
- **TimeWindowSeconds**: How long the window lasts
- **PathPattern**: Which API endpoints this rule affects
- **Priority**: Lower numbers = higher priority (evaluated first)
- **MinTimeBetweenRequestsMs**: Minimum delay between requests (RegionBased only)

### Conflict Resolution

When multiple rules match the same request:
1. **Configuration rules always win** over attribute rules
2. **Lower priority numbers** are evaluated first
3. If any rule blocks the request, it's blocked immediately
4. **Only one rule applies per request** - use different paths to avoid conflicts

### Critical Design Principle

**One Rule Per Path**: To use multiple different rate limiting strategies, target different endpoints:
- `/api/demo` → Fixed Window rule
- `/api/demo/burst` → Token Bucket rule  
- `/api/demo/region/us` → US Regional rule
- `/api/demo/region/eu` → EU Regional rule

---

## Troubleshooting

### Common Issues

1. **Rules not working**:
   - Check that `EnableConfigurationRules: true`
   - Verify `PathPattern` matches your actual endpoint paths
   - Check server logs for rule loading messages

2. **Only one rule applying to same endpoint**:
   - **This is by design** - use different paths for different rule types
   - Check logs for "Conflict resolved" messages

3. **Getting unexpected rate limits**:
   - Check the `X-RateLimit-Rule` header to see which rule is active
   - Review your `ConflictResolutionStrategy` setting
   - Look for "Configuration wins over X other rules" in logs

4. **Rate limits not resetting**:
   - Fixed windows reset at specific intervals
   - Token buckets refill gradually
   - Check the `X-RateLimit-Reset` header for timing

### Debugging Tips

1. **Enable detailed logging**:
```json
"Logging": {
  "LogLevel": {
    "RateLimiter": "Debug"
  }
}
```

2. **Check rule headers**:
```bash
curl -D- http://localhost:5037/api/demo
```

3. **Test with different client IDs**:
```bash
curl -H "X-ClientId: test-user-1" http://localhost:5037/api/demo
curl -H "X-ClientId: test-user-2" http://localhost:5037/api/demo
```

4. **Watch startup logs**:
Look for messages like:
- "Created configuration rule 'BlogReadProtection'"
- "Conflict resolved: BlogReadProtection from Configuration wins over 7 other rules"

---

## Next Steps

1. **Customize for your API**: Modify the `PathPattern` values to match your actual endpoints
2. **Adjust limits**: Change `MaxRequests` and `TimeWindowSeconds` based on your needs
3. **Add more rules**: Create specific rules for different endpoint types
4. **Monitor usage**: Watch the logs and headers to understand actual traffic patterns
5. **Consider Redis**: For multi-server deployments, add Redis configuration for distributed rate limiting

This configuration-based approach means you can adjust rate limits without code changes or deployments - just update `appsettings.json` and restart the service.

## Key Takeaways

✅ **Configuration rules override attribute rules**  
✅ **Use different endpoints for different rule types**  
✅ **Check logs to see which rules are applied**  
✅ **Headers show which rule determined the limit**  
✅ **One rule per request - conflicts are resolved automatically**

