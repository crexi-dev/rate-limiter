using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RateLimiter.Business.Rules;
using RateLimiter.Interfaces.DataAccess;
using RateLimiter.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Business.Rules
{
    [TestClass]
    public class RequestsPerWindowRateLimitRuleTests
    {
        private const string RequestCountKey = "RequestCount";
        private const string WindowStartTimeKey = "WindowStartTime";
        private const string WindowMsKey = "WindowMs";
        private const string RequestLimitKey = "RequestLimit";
        private const string UserId = "CBB9E07A-8015-401D-89EB-7C161BA39297";

        private Mock<IRateLimitRepository> _repository;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IRateLimitRepository>();
        }

        [TestMethod, ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, false)]
        public void InvalidConstructorTest(bool isRepositoryMissing, bool isWindowKeyMissing, bool isLimitKeyMissing)
        {
            var parameters = new Dictionary<string, object>();

            if (!isWindowKeyMissing)
            {
                parameters[WindowMsKey] = 10000u;
            }

            if (!isLimitKeyMissing)
            {
                parameters[RequestLimitKey] = 1000u;
            }

            var repository = isRepositoryMissing ? null : _repository.Object;

            new RequestsPerWindowRateLimitRule(repository, parameters);
        }

        [TestMethod]
        // Initial call should succeeed
        [DataRow(1000u, 100u, true, 0u, 0, true)]
        // Window start time 500ms in the past with 100 previous requests should fail
        [DataRow(1000u, 100u, false, 100u, -500, false)]
        // Window start time 500ms in the past with 99 previous requests should succeed
        [DataRow(1000u, 100u, false, 99u, -500, true)]
        // Over the limit but outside the window, limit should reset and should return true
        [DataRow(1000u, 100u, false, 101u, -2000, true)]
        public async Task VerifyTest(uint windowMs, uint requestLimit, bool isFirstCall, uint requestCount, int startDeltaFromNow, bool expected)
        {
            var now = DateTime.UtcNow;

            var context = new Mock<HttpContext>();

            var user = new Models.User(UserId);

            var path = "/WeatherForecast";
            var verbs = HttpVerbFlags.Get;

            var hash = "KM+g4xjWHlk9dJH1jz00zJ/6vLvF21Z49OQ1RPUDl44=";
            var endpoint = new Mock<IEndpoint>();
            endpoint.Setup(x => x.Verbs).Returns(verbs);
            endpoint.Setup(x => x.PathPattern).Returns(path);

            IDictionary<string, object>? dictionary = null;

            if (!isFirstCall)
            {
                dictionary = new Dictionary<string, object> {
                    {RequestCountKey,  requestCount},
                    {WindowStartTimeKey,  now.AddMilliseconds(startDeltaFromNow)}
                };
            }

            var parameters = new Dictionary<string, object>
            {
                { WindowMsKey, windowMs },
                { RequestLimitKey, requestLimit }
            };

            _repository.Setup(x => x.Retrieve(hash)).Returns(Task.FromResult(dictionary));

            var target = new RequestsPerWindowRateLimitRule(_repository.Object, parameters);

            var actual = await target.Verify(context.Object, user, endpoint.Object, now);

            Assert.AreEqual(expected, actual.Proceed);

            _repository.Verify(x => x.Update(hash, It.IsAny<Dictionary<string, object>>()));
        }
    }
}
