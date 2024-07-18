# Rate Limiter
An implementation of a rules based, configurable, and extendable rate limiter.

## Usage
Can be ran / tested by cloning the repo and running the Unit Tests project.
Can be used by "referencing" the Class Library project.
Built with .NET 8.

## Rate Limiter
The rate limiter is intended to limit access of a "client" to a "resource". The rate limiting algorithm is completely configurable and extendable:

 - Any logic can be used to implement a rate limiting rule, such as "3 requests per minute".
 - Rate limiting rules can be applied to any combination of client / resource, such as "clients which are part of the same company share a limit for any paid resources and have their own limits for other resources."

## Example

    [TestMethod]
    public void RequestsPerTimeMultipleClientsTests()
    {
        TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
    
        RateLimiterBuilder<string, string> rateLimiterBuilder = new([]);
    
        RateLimiter<string, string> rateLimiter =
            rateLimiterBuilder
            .For(x => x.client == "1")
            .Apply(x => [new RequestsPerTimeRule<string, string>(timeProvider, 2, TimeSpan.FromMinutes(1))])
            .For(x => x.client == "2")
            .Apply(x => [new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1))])
            .Build();
    
        rateLimiter.RegisterRequest("1", "");
        rateLimiter.RegisterRequest("1", "");
        bool hasReachedLimit1 = rateLimiter.HasReachedLimit("1", "");
    
        rateLimiter.RegisterRequest("2", "");
        rateLimiter.RegisterRequest("2", "");
        bool hasReachedLimit2 = rateLimiter.HasReachedLimit("2", "");
    
        Assert.IsTrue(hasReachedLimit1);
        Assert.IsFalse(hasReachedLimit2);
    }

## Original Prompt

**Rate-limiting pattern**

Rate limiting involves restricting the number of requests that a client can make.
A client is identified with an access token, which is used for every request to a resource.
To prevent abuse of the server, APIs enforce rate-limiting techniques.
The rate-limiting application can decide whether to allow the request based on the client.
The client makes an API call to a particular resource; the server checks whether the request for this client is within the limit.
If the request is within the limit, then the request goes through.
Otherwise, the API call is restricted.

Some examples of request-limiting rules (you could imagine any others)
* X requests per timespan;
* a certain timespan has passed since the last call;
* For US-based tokens, we use X requests per timespan; for EU-based tokens, a certain timespan has passed since the last call.

The goal is to design a class(-es) that manages each API resource's rate limits by a set of provided *configurable and extendable* rules. For example, for one resource, you could configure the limiter to use Rule A; for another one - Rule B; for a third one - both A + B, etc. Any combination of rules should be possible; keep this fact in mind when designing the classes.

We're more interested in the design itself than in some intelligent and tricky rate-limiting algorithm. There is no need to use a database (in-memory storage is fine) or any web framework. Do not waste time on preparing complex environment, reusable class library covered by a set of tests is more than enough.

There is a Test Project set up for you to use. However, you are welcome to create your own test project and use whatever test runner you like.   

You are welcome to ask any questions regarding the requirements—treat us as product owners, analysts, or whoever knows the business.
If you have any questions or concerns, please submit them as a [GitHub issue](https://github.com/crexi-dev/rate-limiter/issues).

You should [fork](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) the project and [create a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request-from-a-fork) named as `FirstName LastName` once you are finished.

Good luck!
