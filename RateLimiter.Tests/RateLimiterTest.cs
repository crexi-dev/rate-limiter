using System.Threading;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
	    private IRateLimiter _limiter;
	    private string token = "testToken";

	    [SetUp]
	    public void Init()
	    {
			RateLimiter limiter =new RateLimiter(new Store());
			limiter.SetRules(new []{new IntervalRule(), });
			_limiter = limiter;
	    }

        [Test]
        public void Allow()
        {
	        var res1 = _limiter.IsAllowed(token);
            Assert.IsTrue(res1);

			Thread.Sleep(2*1000);
			var res2 = _limiter.IsAllowed(token);

			Assert.IsTrue(res2);
        }

        [Test]
        public void NotAllow()
        {
	        var res1 = _limiter.IsAllowed(token);
	        Assert.IsTrue(res1);

	        var res2 = _limiter.IsAllowed(token);

	        Assert.IsFalse(res2);
        }
	}
}
