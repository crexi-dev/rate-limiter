using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RateLimiter.Extensions;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        IServiceProvider _serviceProvider;
        private IRateLimiterRulesEngine _rulesEngine;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.UseRateLimiter().UseDefaultRateLimiterOptions();
            services.AddSingleton<ICachingService, TestCachingService>();
            services.AddSingleton(typeof(ILogger<>), typeof(DefaultLogService<>));
            services.AddSingleton(typeof(IConfiguration), Configuration.Default);
            _serviceProvider = services.BuildServiceProvider(true);
            IServiceScope scope = _serviceProvider.CreateScope();
            _rulesEngine = scope.ServiceProvider.GetRequiredService<IRateLimiterRulesEngine>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }

        /// <summary>
        /// Check that a second request too close to the original one is rejected by the 
        /// <see cref="TimeSinceLastRequestRule"/>
        /// </summary>
        /// <returns></returns>
        [TestCase(Region.Unknown)]
        [TestCase(Region.EU)]
        public async Task TimeSinceLastRequestRule_TestRejectRequest(Region testRegion)
        {
            var token = new Token("1", new Dictionary<string, string>());
            var location = new Location(":::1", testRegion);
            DateTime currentTime = DateTime.UtcNow;
            // Make two requests very close to each other...
            var request1 = new RequestInfo(token, location, currentTime);
            var request2 = new RequestInfo(token, location, currentTime.AddMilliseconds(5));

            var canProcess1 = await _rulesEngine.CanProcessAsync(request1, CancellationToken.None);
            var canProcess2 = await _rulesEngine.CanProcessAsync(request2, CancellationToken.None);
            Assert.IsTrue(canProcess1);
            Assert.IsFalse(canProcess2);
        }

        /// <summary>
        /// Check that requests are allowed by the <see cref="TimeSinceLastRequestRule"/> if they 
        /// are far enough apart
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TimeSinceLastRequestRule_TestAcceptRequest()
        {
            var token = new Token("1", new Dictionary<string, string>());
            var location = new Location(":::1", Region.EU);
            DateTime currentTime = DateTime.UtcNow;
            // Make two requests very close to each other...
            var request1 = new RequestInfo(token, location, currentTime);
            var request2 = new RequestInfo(token, location, currentTime.AddMilliseconds(10));

            var canProcess1 = await _rulesEngine.CanProcessAsync(request1, CancellationToken.None);
            var canProcess2 = await _rulesEngine.CanProcessAsync(request2, CancellationToken.None);
            Assert.IsTrue(canProcess1);
            Assert.IsTrue(canProcess2);
        }

        [TestCase(Region.Unknown)]
        [TestCase(Region.US)]
        public async Task RequestsPerTimespanRule_TestRejectRequest(Region testRegion)
        {
            var token = new Token("1", new Dictionary<string, string>());
            var location = new Location(":::1", testRegion);
            DateTime currentTime = new DateTime(0); // Starting point
            // Add 9 requests in this time window. Make sure they are all accepted
            for(var i=0; i < 9; i++)
            {
                var request = new RequestInfo(token,location, currentTime.AddMilliseconds((i+1)*10));
                Assert.IsTrue(await _rulesEngine.CanProcessAsync(request, CancellationToken.None));
            }

            // Add a 10th request inside the window
            var badRequest = new RequestInfo(token, location, currentTime.AddMilliseconds(999));
            Assert.IsFalse(await _rulesEngine.CanProcessAsync(badRequest, CancellationToken.None));
        }

        [Test]
        public async Task RequestsPerTimespanRule_TestAcceptRequest()
        {
            var token = new Token("1", new Dictionary<string, string>());
            var location = new Location(":::1", Region.US);
            DateTime currentTime = new DateTime(0); // Starting point
            // Add 9 requests in this time window. Make sure they are all accepted
            for (var i = 0; i < 9; i++)
            {
                var request = new RequestInfo(token, location, currentTime);
                Assert.IsTrue(await _rulesEngine.CanProcessAsync(request, CancellationToken.None));
            }

            // Add a 10th request just outside the window
            var goodRequest = new RequestInfo(token, location, currentTime.AddMilliseconds(1001));
            Assert.IsTrue(await _rulesEngine.CanProcessAsync(goodRequest, CancellationToken.None));
        }

    }
}
