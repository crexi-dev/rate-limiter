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
        private readonly Guid _euGuid = new();
        private readonly Guid _usGuid = new();

        [SetUp]
        public void GlobalSetup()
        {
            RateLimiterSingleton.Instance.AddOrReplaceLimiter(_euGuid, new EuRateLimiter(this._euRate, () => Task.Delay(10)));
            RateLimiterSingleton.Instance.AddOrReplaceLimiter(_usGuid, new UsRateLimiter(this._usResponsesPerSecond, () => Task.Delay(10)));
        }

        [Test]
        public async Task UsTokenShouldBeCalledNotMoreOftenThenRequested()
        {
            await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid);
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            await Task.Delay(500);
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._usGuid));
        }

        [Test]
        public async Task EuTokenShouldBeCalledNotMoreOftenThenRequested()
        {
            await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid);
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
            await Task.Delay(_euRate);
            Assert.IsTrue(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
            Assert.IsFalse(await RateLimiterSingleton.Instance.TryCallAsync(this._euGuid));
        }
    }
}
