** Implementation of Sliding Window Algorithm to rate limit client calls **

Thank you for giving me the opportunity to get this great experience!
In this library project (RateLimiter) I have worked on rate limiting implementation using "Sliding Window Algorithm".
The algorithm implementators are documented with summaries, methods have clear comments. Main conformer method (whick checks if call can be processed or denied
for particular time on timeline period using sliding window, is clear, independent from external rate limiting libraries and is totally reusable! Which we are using
in a testing project below (RateLimiter.Tests), where we mock timestamps and simulate a client call with some test access "token", 
also we save data in a thread-safe in-memory data-structure to associate each sliding window data containing object with caller client by their access token.

I have not a big experience in unit testing, but I hope I have managed to simulate some scenario for different clients to be rate limited, as I commented in test method, on
last case fails, because last client exceeds request count limit and is being rejected.

Thank you!