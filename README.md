***Rate-limiting pattern***

Rate limiting involves restricting the number of requests that a client can make.
A client is identified with an access token, which is used for every request to a resource.
To prevent abuse of the server, APIs enforce rate-limiting techniques.
The rate-limiting application can decide whether to allow the request based on the client.
The client makes an API call to a particular resource; the server checks whether the request for this client is within the limit.
If the request is within the limit, then the request goes through.
Otherwise, the API call is restricted.

**To run the examples:**
run `dotnet run` in RateLimiter

Working example URLs:
- https://localhost:5001/api/test (token bucket rule)
- https://localhost:5001/api/orders (fixed window rule)
- https://localhost:5001/api/orders/item (region based rule)
- https://localhost:5001/api/users (composite rule)

**To run tests:**
run `dotnet test` in RateLimiter.Tests