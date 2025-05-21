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