using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        async Task<bool> ExecWith(RateLimiterMiddleware middleware)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("token", "test");
            context.TraceIdentifier = Guid.NewGuid().ToString();

            await middleware.InvokeAsync(context);

            return context.Response.StatusCode == 200;
        }

        RateLimiterMiddleware MiddlewareWithRules(List<ILimitRule> rules)
        {
            return new RateLimiterMiddleware(async (context) => { }, new RateLimitOptions
            {
                LimitRules = rules
            });
        }

        [Test]
        public void SendThreeRequests_WithLimitOfTwoInHour_ThirdShouldBeRejected()
        {
            RateLimiterMiddleware middleware = MiddlewareWithRules(new List<ILimitRule>
            {
                new RequestsPerTimespan(time: TimeSpan.FromHours(1), count: 2)
            });

            Assert.Multiple(async () =>
            {
                Assert.IsTrue(await ExecWith(middleware));
                Assert.IsTrue(await ExecWith(middleware));
                Assert.IsFalse(await ExecWith(middleware));
            });
        }

        [Test]
        public async Task SendMultipleRequestsWithMultipleRules_WithLimitOfTenInHourAndLimitOfFiveInFiveSeconds_LimitOfTenShouldReject()
        {
            RateLimiterMiddleware middleware = MiddlewareWithRules(new List<ILimitRule>
            {
                new RequestsPerTimespan(time: TimeSpan.FromHours(1), count: 10),
                new RequestsPerTimespan(time: TimeSpan.FromSeconds(5), count: 5)
            });

            await ExecWith(middleware);
            await ExecWith(middleware);
            await ExecWith(middleware);
            await ExecWith(middleware);

            Thread.Sleep(6000);

            await ExecWith(middleware);
            await ExecWith(middleware);
            await ExecWith(middleware);
            await ExecWith(middleware);

            Thread.Sleep(6000);

            await ExecWith(middleware);
            await ExecWith(middleware);
            Assert.IsFalse(await ExecWith(middleware));
        }

    }
}
