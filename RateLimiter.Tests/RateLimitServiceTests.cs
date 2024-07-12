using FakeItEasy;
using NUnit.Framework;
using RateLimiter.Data;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimitServiceTests
{
    private readonly IRequestTrafficDataAccess fakeRequestTrafficDataAccess;

    /// <summary>
    ///     Can use 1 time setup but even XUnit doesn't even need it.
    ///     Just take simple constructor approach.
    /// </summary>
    public RateLimitServiceTests()
	{
        fakeRequestTrafficDataAccess = A.Fake<IRequestTrafficDataAccess>();
	}

    [Test]
	public void Example()
	{
		Assert.That(true, Is.True);
	}
}