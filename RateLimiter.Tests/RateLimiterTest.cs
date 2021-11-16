using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private readonly int _euRate = 2000;
        private readonly int _usResponsesPerSecond = 2;
        private readonly Guid _euGuid = Guid.NewGuid();
        private readonly Guid _usGuid = Guid.NewGuid();

        [SetUp]
        public void GlobalSetup()
        {
            RateLimiterSingleton.Instance.AddOrReplaceLimiter(_euGuid, new EuRateLimiter(this._euRate, () => Task.Delay(10)));
            RateLimiterSingleton.Instance.AddOrReplaceLimiter(_usGuid, new UsRateLimiter(this._usResponsesPerSecond, () => Task.Delay(10)));
        }

        [Test]
        public async Task UsTokenShouldBeCalledNotMoreOftenThenRequested()
        {
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            await Task.Delay(1000);
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
        }

        [Test]
        public async Task EuTokenShouldBeCalledNotMoreOftenThenRequested()
        {
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
            await Task.Delay(_euRate);
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
        }
    }
}
