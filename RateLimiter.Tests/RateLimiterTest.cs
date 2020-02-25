using NUnit.Framework;
using RateLimiter.Limiter;
using System.Linq;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void Example()
        {
            LimitResolver.Instance.AddLimit("US", 10, 3); // A type - 3 requests in 10 seconds

            LimitResolver.Instance.AddTokenLimit("A", "US");

            Enumerable.Range(1, 4).AsParallel().ForAll(t =>
             {
                 Assert.IsTrue((t == 4) != LimitResolver.Instance.NewQuery("A"));
             });
            Thread.Sleep(3000000);
            Assert.IsTrue(LimitResolver.Instance.NewQuery("A"));
        }
    }
}
