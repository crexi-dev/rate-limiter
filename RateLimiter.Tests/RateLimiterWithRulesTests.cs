using RateLimiter.RateLimitingRules;
using Microsoft.Extensions.Time.Testing;

namespace RateLimiter.Tests;

[TestClass]
public class RateLimiterWithRulesTests
{
    [TestMethod]
    public void RequestsPerTimeTests()
    {
        TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1))])]));

        rateLimiter.RegisterRequest("", "");
        rateLimiter.RegisterRequest("", "");
        bool hasReachedLimit = rateLimiter.HasReachedLimit("", "");

        Assert.IsFalse(hasReachedLimit);
    }

    [TestMethod]
    public void RequestsPerTimeMultipleClientsTests()
    {
        TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1))])]));

        rateLimiter.RegisterRequest("1", "");
        rateLimiter.RegisterRequest("1", "");
        bool hasReachedLimit1 = rateLimiter.HasReachedLimit("1", "");

        rateLimiter.RegisterRequest("2", "");
        rateLimiter.RegisterRequest("2", "");
        bool hasReachedLimit2 = rateLimiter.HasReachedLimit("2", "");

        Assert.IsFalse(hasReachedLimit1);
        Assert.IsFalse(hasReachedLimit2);
    }

    [TestMethod]
    public void RequestsPerTimeMultipleClientsSharedTests()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var rule = new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1));
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [rule])]));

        rateLimiter.RegisterRequest("1", "");
        rateLimiter.RegisterRequest("1", "");
        bool hasReachedLimit1 = rateLimiter.HasReachedLimit("1", "");
        bool hasReachedLimit2 = rateLimiter.HasReachedLimit("2", "");
        bool hasReachedLimit3 = rateLimiter.HasReachedLimit("3", "");

        timeProvider.Advance(TimeSpan.FromMinutes(0.5));

        rateLimiter.RegisterRequest("2", "");
        rateLimiter.RegisterRequest("2", "");
        bool hasReachedLimit1a = rateLimiter.HasReachedLimit("1", "");
        bool hasReachedLimit2a = rateLimiter.HasReachedLimit("2", "");
        bool hasReachedLimit3a = rateLimiter.HasReachedLimit("3", "");

        timeProvider.Advance(TimeSpan.FromMinutes(0.5));

        bool hasReachedLimit1b = rateLimiter.HasReachedLimit("1", "");
        bool hasReachedLimit2b = rateLimiter.HasReachedLimit("2", "");
        bool hasReachedLimit3b = rateLimiter.HasReachedLimit("3", "");

        Assert.IsFalse(hasReachedLimit1);
        Assert.IsFalse(hasReachedLimit2);
        Assert.IsFalse(hasReachedLimit3);

        Assert.IsTrue(hasReachedLimit1a);
        Assert.IsTrue(hasReachedLimit2a);
        Assert.IsTrue(hasReachedLimit3a);

        Assert.IsFalse(hasReachedLimit1b);
        Assert.IsFalse(hasReachedLimit2b);
        Assert.IsFalse(hasReachedLimit3b);
    }

    [TestMethod]
    public void TimeSinceRequestTests()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [new TimeSinceLastRequestRule<string, string>(timeProvider, TimeSpan.FromMinutes(1))])]));

        bool hasReachedLimit = rateLimiter.HasReachedLimit("", "");
        rateLimiter.RegisterRequest("", "");
        bool hasReachedLimit2 = rateLimiter.HasReachedLimit("", "");
        timeProvider.Advance(TimeSpan.FromMinutes(1));
        bool hasReachedLimit3 = rateLimiter.HasReachedLimit("", "");

        Assert.IsFalse(hasReachedLimit);
        Assert.IsTrue(hasReachedLimit2);
        Assert.IsFalse(hasReachedLimit3);
    }

    record Company(string Name, int RequestsPerMinute);
    record CompanyToken(string Value, Company Company);
    [TestMethod]
    public void RequestPerTimeCompanySharedTest()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        DynamicDictionary<Company, RequestsPerTimeRule<CompanyToken, string>> rulesPerCompany = new([(x => true, x => [new RequestsPerTimeRule<CompanyToken, string>(timeProvider, x.RequestsPerMinute, TimeSpan.FromMinutes(1))])]);
        RateLimiter<CompanyToken, string> rateLimiter = new(new([(x => true, x => rulesPerCompany.Get(x.client.Company))]));

        Company companyA = new Company("Company A", 4);
        Company companyB = new Company("Company B", 6);

        CompanyToken companyAUser1 = new CompanyToken("companyAUser1", companyA);
        CompanyToken companyAUser2 = new CompanyToken("companyAUser2", companyA);
        CompanyToken companyBUser1 = new CompanyToken("companyBUser1", companyB);
        CompanyToken companyBUser2 = new CompanyToken("companyBUser2", companyB);
        CompanyToken companyBUser3 = new CompanyToken("companyBUser3", companyB);

        bool hasReachedLimitCompanyAUser1A = rateLimiter.HasReachedLimit(companyAUser1, "");
        bool hasReachedLimitCompanyAUser2A = rateLimiter.HasReachedLimit(companyAUser2, "");
        bool hasReachedLimitCompanyBUser1A = rateLimiter.HasReachedLimit(companyBUser1, "");
        bool hasReachedLimitCompanyBUser2A = rateLimiter.HasReachedLimit(companyBUser2, "");
        bool hasReachedLimitCompanyBUser3A = rateLimiter.HasReachedLimit(companyBUser3, "");

        rateLimiter.RegisterRequest(companyAUser1, "");
        rateLimiter.RegisterRequest(companyAUser2, "");
        rateLimiter.RegisterRequest(companyBUser1, "");
        rateLimiter.RegisterRequest(companyBUser2, "");
        rateLimiter.RegisterRequest(companyBUser3, "");

        timeProvider.Advance(TimeSpan.FromMinutes(0.5));

        bool hasReachedLimitCompanyAUser1B = rateLimiter.HasReachedLimit(companyAUser1, "");
        bool hasReachedLimitCompanyAUser2B = rateLimiter.HasReachedLimit(companyAUser2, "");
        bool hasReachedLimitCompanyBUser1B = rateLimiter.HasReachedLimit(companyBUser1, "");
        bool hasReachedLimitCompanyBUser2B = rateLimiter.HasReachedLimit(companyBUser2, "");
        bool hasReachedLimitCompanyBUser3B = rateLimiter.HasReachedLimit(companyBUser3, "");

        rateLimiter.RegisterRequest(companyAUser1, "");
        rateLimiter.RegisterRequest(companyAUser2, "");
        rateLimiter.RegisterRequest(companyBUser1, "");
        rateLimiter.RegisterRequest(companyBUser2, "");
        rateLimiter.RegisterRequest(companyBUser3, "");

        bool hasReachedLimitCompanyAUser1C = rateLimiter.HasReachedLimit(companyAUser1, "");
        bool hasReachedLimitCompanyAUser2C = rateLimiter.HasReachedLimit(companyAUser2, "");
        bool hasReachedLimitCompanyBUser1C = rateLimiter.HasReachedLimit(companyBUser1, "");
        bool hasReachedLimitCompanyBUser2C = rateLimiter.HasReachedLimit(companyBUser2, "");
        bool hasReachedLimitCompanyBUser3C = rateLimiter.HasReachedLimit(companyBUser3, "");

        timeProvider.Advance(TimeSpan.FromMinutes(0.5));

        bool hasReachedLimitCompanyAUser1D = rateLimiter.HasReachedLimit(companyAUser1, "");
        bool hasReachedLimitCompanyAUser2D = rateLimiter.HasReachedLimit(companyAUser2, "");
        bool hasReachedLimitCompanyBUser1D = rateLimiter.HasReachedLimit(companyBUser1, "");
        bool hasReachedLimitCompanyBUser2D = rateLimiter.HasReachedLimit(companyBUser2, "");
        bool hasReachedLimitCompanyBUser3D = rateLimiter.HasReachedLimit(companyBUser3, "");

        rateLimiter.RegisterRequest(companyBUser1, "");
        rateLimiter.RegisterRequest(companyBUser2, "");
        rateLimiter.RegisterRequest(companyBUser3, "");

        bool hasReachedLimitCompanyAUser1E = rateLimiter.HasReachedLimit(companyAUser1, "");
        bool hasReachedLimitCompanyAUser2E = rateLimiter.HasReachedLimit(companyAUser2, "");
        bool hasReachedLimitCompanyBUser1E = rateLimiter.HasReachedLimit(companyBUser1, "");
        bool hasReachedLimitCompanyBUser2E = rateLimiter.HasReachedLimit(companyBUser2, "");
        bool hasReachedLimitCompanyBUser3E = rateLimiter.HasReachedLimit(companyBUser3, "");

        Assert.IsFalse(hasReachedLimitCompanyAUser1A);
        Assert.IsFalse(hasReachedLimitCompanyAUser2A);
        Assert.IsFalse(hasReachedLimitCompanyBUser1A);
        Assert.IsFalse(hasReachedLimitCompanyBUser2A);
        Assert.IsFalse(hasReachedLimitCompanyBUser3A);

        Assert.IsFalse(hasReachedLimitCompanyAUser1B);
        Assert.IsFalse(hasReachedLimitCompanyAUser2B);
        Assert.IsFalse(hasReachedLimitCompanyBUser1B);
        Assert.IsFalse(hasReachedLimitCompanyBUser2B);
        Assert.IsFalse(hasReachedLimitCompanyBUser3B);

        Assert.IsTrue(hasReachedLimitCompanyAUser1C);
        Assert.IsTrue(hasReachedLimitCompanyAUser2C);
        Assert.IsTrue(hasReachedLimitCompanyBUser1C);
        Assert.IsTrue(hasReachedLimitCompanyBUser2C);
        Assert.IsTrue(hasReachedLimitCompanyBUser3C);

        Assert.IsFalse(hasReachedLimitCompanyAUser1D);
        Assert.IsFalse(hasReachedLimitCompanyAUser2D);
        Assert.IsFalse(hasReachedLimitCompanyBUser1D);
        Assert.IsFalse(hasReachedLimitCompanyBUser2D);
        Assert.IsFalse(hasReachedLimitCompanyBUser3D);

        Assert.IsFalse(hasReachedLimitCompanyAUser1E);
        Assert.IsFalse(hasReachedLimitCompanyAUser2E);
        Assert.IsTrue(hasReachedLimitCompanyBUser1E);
        Assert.IsTrue(hasReachedLimitCompanyBUser2E);
        Assert.IsTrue(hasReachedLimitCompanyBUser3E);
    }
}
