using NUnit.Framework;
using RateLimiter.Models;
using System.Threading.Tasks;
using System;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [TestCase("api-key-US-1")]
        public void VerifyRateLimitIsHit_HeaderStrategy(string key)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var request = new Request(key, "", "");
                    request.Timestamp = DateTime.UtcNow;

                    //** ACT **
                    rateLimiter.ValidateRateLimit_Header(request);
                    Log($"{key}-{i}");
                }
                // Failsafe
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown at {DateTime.Now:HH:mm:ss.ff}");

                //** ASSERT ** 
                Assert.IsTrue(ex.GetType() == typeof(RateLimitException));
            }
        }

        [TestCase("api-key-US")]
        public void VerifyRateLimitIsHitButSecondRequestWorks_HeaderStrategy(string key)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();
            var request = new Request(key, "", "");
            var request1 = new Request($"{key}-new1", "", "");
            var request2 = new Request($"{key}-new2", "", "");

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    request.Timestamp = DateTime.UtcNow;

                    //** ACT **
                    rateLimiter.ValidateRateLimit_Header(request);
                    Log($"{key}-{i}");
                }
                // Failsafe
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown at {DateTime.Now:HH:mm:ss.ff}");

                //** ASSERT ** 
                request1.Timestamp = DateTime.UtcNow;
                Assert.IsTrue(rateLimiter.ValidateRateLimit_Header(request1));
                Log($"{key}-new1 - WORKS");

                request2.Timestamp = DateTime.UtcNow;
                Assert.IsTrue(rateLimiter.ValidateRateLimit_Header(request2));
                Log($"{key}-new2 - WORKS");

                request.Timestamp = DateTime.UtcNow;
                Log($"{key}-original");
                Assert.Throws<RateLimitException>(() => rateLimiter.ValidateRateLimit_Header(request));
                Console.WriteLine($"Original fails after trying again at {DateTime.Now:HH:mm:ss.ff}");
            }
        }

        [TestCase("api-key-EU-1", 10)]
        [TestCase("api-key-EU-2", 5)]
        public async Task VerifyRateLimitIsNotHitWhenStaggered_HeaderStrategy(string key, int numOfRequests = 0)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();
            var numOfRequestsCompleted = 0;

            for (int i = 0; i < numOfRequests; i++)
            {
                var request = new Request(key, "", "");
                request.Timestamp = DateTime.UtcNow;

                //** ACT **
                Log($"{key}-{i}");
                var result = rateLimiter.ValidateRateLimit_Header(request);
                if (result)
                {
                    numOfRequestsCompleted++;
                }
                await Task.Delay(2001);
            }
            // ** ASSERT **
            Assert.IsTrue(numOfRequestsCompleted == numOfRequests);
        }

        [TestCase("api-key-US-1", 100)]
        [TestCase("api-key-EU-1", 5)]
        public void VerifyRateLimitIsHitWhenNotStaggered_HeaderStrategy(string key, int numOfRequests = 0)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();
            var numOfRequestsCompleted = 0;

            try
            {
                for (int i = 0; i < numOfRequests; i++)
                {
                    var request = new Request(key, "", "");
                    request.Timestamp = DateTime.UtcNow;

                    //** ACT **
                    var result = rateLimiter.ValidateRateLimit_Header(request);
                    Log($"{key}-{i}");
                    if (result)
                    {
                        numOfRequestsCompleted++;
                    }
                }
                // Failsafe
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown at {DateTime.Now:HH:mm:ss.ff}");

                //** ASSERT ** 
                Assert.IsTrue(ex.GetType() == typeof(RateLimitException));
            }
        }

        [TestCase("api/users", 100)]
        [TestCase("api/bid", 5000)]
        public void VerifyRateLimitIsHitWhenNotStaggered_EndpointStrategy(string endpoint, int numOfRequests = 0)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();
            var numOfRequestsCompleted = 0;

            try
            {
                for (int i = 0; i < numOfRequests; i++)
                {
                    var request = new Request("", "", endpoint);
                    request.Timestamp = DateTime.UtcNow;

                    //** ACT **
                    var result = rateLimiter.ValidateRateLimit_Endpoint(request);
                    Log($"{endpoint}-{i}");
                    if (result)
                    {
                        numOfRequestsCompleted++;
                    }
                }
                // Failsafe
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown at {DateTime.Now:HH:mm:ss.ff}");

                //** ASSERT ** 
                Assert.IsTrue(ex.GetType() == typeof(RateLimitException));
            }
        }

        [TestCase("api/users", 10)]
        [TestCase("api/bid", 5)]
        public async Task VerifyRateLimitIsNotHitWhenStaggered_EndpointStrategy(string key, int numOfRequests = 0)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();
            var numOfRequestsCompleted = 0;

            for (int i = 0; i < numOfRequests; i++)
            {
                var request = new Request(key, "", "");
                request.Timestamp = DateTime.UtcNow;

                //** ACT **
                Log($"{key}-{i}");
                var result = rateLimiter.ValidateRateLimit_Endpoint(request);
                if (result)
                {
                    numOfRequestsCompleted++;
                }
                await Task.Delay(2001);
            }
            // ** ASSERT **
            Assert.IsTrue(numOfRequestsCompleted == numOfRequests);
        }

        [TestCase("api-key-US-1", 100)]
        [TestCase("api/bid", 5000)]
        public void VerifyRateLimitIsHitWhenNotStaggered_MultipleStrategy(string guard, int numOfRequests = 0)
        {
            //** ARRANGE **
            var rateLimiter = new RateLimiter();
            var numOfRequestsCompleted = 0;

            try
            {
                for (int i = 0; i < numOfRequests; i++)
                {
                    var request = new Request("", "", "");
                    if (guard == "api/bid")
                    {
                        request.Destination = guard;
                    }
                    else
                    {
                        request.Key = guard;
                    }
                    request.Timestamp = DateTime.UtcNow;

                    //** ACT **
                    var result = rateLimiter.ValidateRateLimit_Multiple(request);
                    Log($"{guard}-{i}");
                    if (result)
                    {
                        numOfRequestsCompleted++;
                    }
                }
                // Failsafe
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Thrown at {DateTime.Now:HH:mm:ss.ff}");

                //** ASSERT ** 
                Assert.IsTrue(ex.GetType() == typeof(RateLimitException));
            }
        }

        private static void Log(string key)
        {
            Console.WriteLine($"Submitting Request - {key} at {DateTime.Now:HH:mm:ss.ff}");
        }
    }
}
