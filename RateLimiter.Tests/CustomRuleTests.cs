using Microsoft.Extensions.Time.Testing;
using RateLimiter.RateLimitingRules;

namespace RateLimiter.Tests;

[TestClass]
public class CustomRuleTests
{
    enum Location { US, EU };
    record LocationToken(string Value, Location Location);
    class RequestsPerTimeForUSTimeSinceLastRequestForEU(RequestsPerTimeRule<LocationToken, string> requestsPerTimeRule, TimeSinceLastRequestRule<LocationToken, string> timeSinceLastRequestRule) : IRateLimitingRule<LocationToken, string>
    {
        public bool HasReachedLimit(LocationToken client, string resource) => client.Location switch
        {
            Location.US => requestsPerTimeRule.HasReachedLimit(client, resource),
            Location.EU => timeSinceLastRequestRule.HasReachedLimit(client, resource),
            _ => throw new Exception(),
        };

        public void RegisterRequest(LocationToken client, string resource)
        {
            switch (client.Location)
            {
                case Location.US: requestsPerTimeRule.RegisterRequest(client, resource); break;
                case Location.EU: timeSinceLastRequestRule.RegisterRequest(client, resource); break;
                default: throw new Exception();
            }
        }
    }

    [TestMethod]
    public void RequestsPerTimeForUSTimeSinceLastRequestForEUWithCustomRuleTest()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<LocationToken, string> rateLimiter = new(new([(x => true, x => [new RequestsPerTimeForUSTimeSinceLastRequestForEU(new(timeProvider, 3, TimeSpan.FromMinutes(1)), new(timeProvider, TimeSpan.FromMinutes(1)))])]));

        LocationToken usToken = new("", Location.US);
        LocationToken euToken = new("", Location.EU);

        rateLimiter.RegisterRequest(usToken, "");
        rateLimiter.RegisterRequest(usToken, "");

        bool hasReachedLimitUs = rateLimiter.HasReachedLimit(usToken, "");
        bool hasReachedLimitEu = rateLimiter.HasReachedLimit(euToken, "");

        rateLimiter.RegisterRequest(usToken, "");
        rateLimiter.RegisterRequest(euToken, "");

        bool hasReachedLimitUsA = rateLimiter.HasReachedLimit(usToken, "");
        bool hasReachedLimitEuA = rateLimiter.HasReachedLimit(euToken, "");

        timeProvider.Advance(TimeSpan.FromMinutes(1));

        bool hasReachedLimitUsB = rateLimiter.HasReachedLimit(usToken, "");
        bool hasReachedLimitEuB = rateLimiter.HasReachedLimit(euToken, "");

        Assert.IsFalse(hasReachedLimitUs);
        Assert.IsFalse(hasReachedLimitEu);

        Assert.IsTrue(hasReachedLimitUsA);
        Assert.IsTrue(hasReachedLimitEuA);

        Assert.IsFalse(hasReachedLimitUsB);
        Assert.IsFalse(hasReachedLimitEuB);
    }

    [TestMethod]
    public void RequestsPerTimeForUSTimeSinceLastRequestForEUWithoutCustomRuleTest()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<LocationToken, string> rateLimiter = new(new(
        [
            (x => x.client.Location == Location.US, x => [new RequestsPerTimeRule<LocationToken, string>(timeProvider, 3, TimeSpan.FromMinutes(1))]),
            (x => x.client.Location == Location.EU, x => [new TimeSinceLastRequestRule<LocationToken, string>(timeProvider, TimeSpan.FromMinutes(1))])
        ]));

        LocationToken usToken = new("", Location.US);
        LocationToken euToken = new("", Location.EU);

        rateLimiter.RegisterRequest(usToken, "");
        rateLimiter.RegisterRequest(usToken, "");

        bool hasReachedLimitUs = rateLimiter.HasReachedLimit(usToken, "");
        bool hasReachedLimitEu = rateLimiter.HasReachedLimit(euToken, "");

        rateLimiter.RegisterRequest(usToken, "");
        rateLimiter.RegisterRequest(euToken, "");

        bool hasReachedLimitUsA = rateLimiter.HasReachedLimit(usToken, "");
        bool hasReachedLimitEuA = rateLimiter.HasReachedLimit(euToken, "");

        timeProvider.Advance(TimeSpan.FromMinutes(1));

        bool hasReachedLimitUsB = rateLimiter.HasReachedLimit(usToken, "");
        bool hasReachedLimitEuB = rateLimiter.HasReachedLimit(euToken, "");

        Assert.IsFalse(hasReachedLimitUs);
        Assert.IsFalse(hasReachedLimitEu);

        Assert.IsTrue(hasReachedLimitUsA);
        Assert.IsTrue(hasReachedLimitEuA);

        Assert.IsFalse(hasReachedLimitUsB);
        Assert.IsFalse(hasReachedLimitEuB);
    }

    [TestMethod]
    public void RequestsPerTimeForUSTimeSinceLastRequestForEUWithoutCustomRuleForNonAdminResourcesTest()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<LocationToken, string> rateLimiter = new(new(
        [
            (x => x.client.Location == Location.US && !x.resource.Contains("/admin"), x => [new RequestsPerTimeRule<LocationToken, string>(timeProvider, 3, TimeSpan.FromMinutes(1))]),
            (x => x.client.Location == Location.EU && !x.resource.Contains("/admin"), x => [new TimeSinceLastRequestRule<LocationToken, string>(timeProvider, TimeSpan.FromMinutes(1))])
        ]));

        LocationToken usToken = new("", Location.US);
        LocationToken euToken = new("", Location.EU);

        rateLimiter.RegisterRequest(usToken, "resource");
        rateLimiter.RegisterRequest(usToken, "resource");
        rateLimiter.RegisterRequest(usToken, "/admin/resource");
        rateLimiter.RegisterRequest(usToken, "/admin/resource");

        bool hasReachedLimitUs = rateLimiter.HasReachedLimit(usToken, "resource");
        bool hasReachedLimitEu = rateLimiter.HasReachedLimit(euToken, "resource");
        bool hasReachedLimitUsAdmin = rateLimiter.HasReachedLimit(usToken, "/admin/resource");
        bool hasReachedLimitEuAdmin = rateLimiter.HasReachedLimit(euToken, "/admin/resource");

        rateLimiter.RegisterRequest(usToken, "resource");
        rateLimiter.RegisterRequest(euToken, "resource");
        rateLimiter.RegisterRequest(usToken, "/admin/resource");
        rateLimiter.RegisterRequest(euToken, "/admin/resource");

        bool hasReachedLimitUsA = rateLimiter.HasReachedLimit(usToken, "resource");
        bool hasReachedLimitEuA = rateLimiter.HasReachedLimit(euToken, "resource");
        bool hasReachedLimitUsAAdmin = rateLimiter.HasReachedLimit(usToken, "/admin/resource");
        bool hasReachedLimitEuAAdmin = rateLimiter.HasReachedLimit(euToken, "/admin/resource");

        timeProvider.Advance(TimeSpan.FromMinutes(1));

        bool hasReachedLimitUsB = rateLimiter.HasReachedLimit(usToken, "resource");
        bool hasReachedLimitEuB = rateLimiter.HasReachedLimit(euToken, "resource");
        bool hasReachedLimitUsBAdmin = rateLimiter.HasReachedLimit(usToken, "/admin/resource");
        bool hasReachedLimitEuBAdmin = rateLimiter.HasReachedLimit(euToken, "/admin/resource");

        Assert.IsFalse(hasReachedLimitUs);
        Assert.IsFalse(hasReachedLimitEu);

        Assert.IsTrue(hasReachedLimitUsA);
        Assert.IsTrue(hasReachedLimitEuA);

        Assert.IsFalse(hasReachedLimitUsB);
        Assert.IsFalse(hasReachedLimitEuB);


        Assert.IsFalse(hasReachedLimitUsAdmin);
        Assert.IsFalse(hasReachedLimitEuAdmin);

        Assert.IsFalse(hasReachedLimitUsAAdmin);
        Assert.IsFalse(hasReachedLimitEuAAdmin);

        Assert.IsFalse(hasReachedLimitUsBAdmin);
        Assert.IsFalse(hasReachedLimitEuBAdmin);
    }
}
