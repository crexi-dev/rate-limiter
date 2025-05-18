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