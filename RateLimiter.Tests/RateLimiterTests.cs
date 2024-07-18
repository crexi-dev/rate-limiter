namespace RateLimiter.Tests;

[TestClass]
public class RateLimiterTests
{
    [TestMethod]
    public void HasReachedLimitIsFalseWhenNoConfigurationAndNoRequests()
    {
        RateLimiter<string, string> rateLimiter = new(new ([]));

        bool hasReachedLimit = rateLimiter.HasReachedLimit("", "");

        Assert.IsFalse(hasReachedLimit);
    }

    [TestMethod]
    public void RegisterRequestWhenNoConfiguration()
    {
        RateLimiter<string, string> rateLimiter = new(new([]));

        rateLimiter.RegisterRequest("", "");
    }

    [TestMethod]
    public void HasReachedLimitIsFalseWhenNoConfigurationAndSomeRequests()
    {
        RateLimiter<string, string> rateLimiter = new(new([]));

        rateLimiter.RegisterRequest("", "");
        rateLimiter.RegisterRequest("", "");
        rateLimiter.RegisterRequest("", "");
        bool hasReachedLimit = rateLimiter.HasReachedLimit("", "");

        Assert.IsFalse(hasReachedLimit);
    }
}