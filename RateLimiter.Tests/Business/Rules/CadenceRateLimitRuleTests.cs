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
    public class CadenceRateLimitRuleTests
    {
        private const string LastAccessedKey = "LastAccessed";
        private const string MinimumDelayBetweenRequestsMsKey = "MinimumDelayBetweenRequestsMs";
        private const string UserId = "CBB9E07A-8015-401D-89EB-7C161BA39297";

        private Mock<IRateLimitRepository> _repository;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IRateLimitRepository>();
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void MissingParametersConstructorTest()
        {
            var parameters = new Dictionary<string, object>();
            var target = new CadenceRateLimitRule(_repository.Object, parameters);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void MissingRepositoryConstructorTest()
        {
            var parameters = new Dictionary<string, object> { { MinimumDelayBetweenRequestsMsKey, 10u } };

            var target = new CadenceRateLimitRule(null, parameters);
        }

        [TestMethod]
        // No previous requests/expect true
        [DataRow(10u, null, true)]
        // Not enough time between the current and last request
        [DataRow(5000u, -1000, false)]
        // Sufficient time between the current and last request
        [DataRow(5000u, -10000, true)]
        public async Task VerifyTest(uint minimumDelayBetweenRequestsMs, int? lastAccessedDeltaMs, bool expected)
        {
            var now = DateTime.UtcNow;

            var context = new Mock<HttpContext>();

            var user = new Models.User(UserId);

            var path = "/WeatherForecast";
            var verbs = HttpVerbFlags.Get;

            var hash = "yuNEZDhOtAnGt7O/ZvE0ahtkM7IFm9nNE0LAxQIRhsU=";
            var endpoint = new Mock<IEndpoint>();

            endpoint.Setup(x => x.Verbs).Returns(verbs);
            endpoint.Setup(x => x.PathPattern).Returns(path);

            var dictionary = lastAccessedDeltaMs == null
                ? null as IDictionary<string, object>
                : new Dictionary<string, object> { { LastAccessedKey, now.AddMilliseconds(lastAccessedDeltaMs!.Value) } };

            var parameters = new Dictionary<string, object>
            {
                { MinimumDelayBetweenRequestsMsKey, minimumDelayBetweenRequestsMs}
            };

            _repository.Setup(x => x.Retrieve(hash)).Returns(Task.FromResult(dictionary));

            var target = new CadenceRateLimitRule(_repository.Object, parameters);

            var actual = await target.Verify(context.Object, user, endpoint.Object, now);

            Assert.AreEqual(expected, actual.Proceed);

            _repository.Verify(x => x.Update(hash, It.IsAny<Dictionary<string, object>>()));
        }
    }
}
