using NUnit.Framework;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Services;
using RateLimiter.Interfaces;

namespace RateLimiter.Tests.Unit
{
    [TestFixture]
    public class InMemoryRepositoryTests
    {
        [Test]
        public void CountRequests_Returns10_When10UserRequestsAdded()
        {
            var requestDate = Substitute.For<IDateTimeWrapper>();
            requestDate.Now = new DateTime(2022, 2, 2, 2, 2, 2, 2);

            var repository = new RateLimiterInMemoryRepository();

            for (int i = 0; i < 10; i++)
            {
                //the request we're counting
                repository.SaveUserRequest("a", requestDate);

                //another user's request
                repository.SaveUserRequest("b", requestDate);

            }

            //count requests 1 second back
            var result = repository.CountRequests("a", requestDate, TimeSpan.FromSeconds(1));

            Assert.IsTrue(result == 10);
        }

        [Test]
        public void Cleanup_ClearsExpiredRequests_WhenCalled()
        {
            var requestDate = Substitute.For<IDateTimeWrapper>();
            requestDate.Now = new DateTime(2022, 2, 2, 2, 2, 2, 2);

            var repository = new RateLimiterInMemoryRepository();

            for (int i = 0; i < 10; i++)
            {
                //the request we're counting
                repository.SaveUserRequest("a", requestDate);

                //another user's request
                repository.SaveUserRequest("b", requestDate);

            }

            repository.Cleanup(requestDate.Now.AddDays(2));

            //count requests 1 second back
            var result = repository.CountRequests("a", requestDate, TimeSpan.FromSeconds(1));

            Assert.IsTrue(result == 0);
        }
    }
}
