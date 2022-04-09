using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Services;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private IRateLimiterService _rateLimiterService = new RateLimiterService();

        /// <summary>
        /// First any Client's request positive test
        /// </summary>
        [Test, Order(1)]
        public void Test_RateLimiterService_FirstEntry()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientBToken, DataStorageSimulator.AvailableResource.ResourceOne);
            Assert.IsTrue(result);
        }

        /// <summary>
        /// ClientB - rlRequestsPerInterval - Negative test
        /// </summary>
        [Test, Order(4)]
        public void Test_RateLimiterService_ToManyRequests_Per_Interval()
        {
            Thread.Sleep(2000);
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientBToken, DataStorageSimulator.AvailableResource.ResourceTwo);
            for (int i = 0; i < 15; i++)
            {
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientBToken, DataStorageSimulator.AvailableResource.ResourceTwo);
                if (!result) 
                {
                    break;
                }
            }
            Assert.IsFalse(result);
        }
        /// <summary>
        /// ClientB - rlRequestsPerInterval -  positive test ResourceTwo
        /// </summary>
        [Test, Order(2)]
        public void Test_RateLimiterService_Acceptable_Rate_Per_Interval_ResourceTwo()
        {
            bool result = false;
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(800);
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientBToken, DataStorageSimulator.AvailableResource.ResourceTwo);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsTrue(result);
        }

        /// <summary>
        /// ClientB - rlRequestsPerInterval -  positive test ResourceThree
        /// </summary>
        [Test, Order(3)]
        public void Test_RateLimiterService_Acceptable_Rate_Per_Interval_ResourceThree()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientBToken, DataStorageSimulator.AvailableResource.ResourceThree);
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(800);
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientBToken, DataStorageSimulator.AvailableResource.ResourceThree);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsTrue(result);
        }
        /// <summary>
        /// ClientA - rlRequestsPerTimeout - negative test
        /// </summary>
        [Test]
        public void Test_RateLimiterService_Fast_Requests_Rate_Per_Timeout()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientAToken, DataStorageSimulator.AvailableResource.ResourceOne);
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(100);
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientAToken, DataStorageSimulator.AvailableResource.ResourceOne);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsFalse(result);
        }
        /// <summary>
        /// ClientA - rlRequestsPerTimeout - positive test
        /// </summary>
        [Test]
        public void Test_RateLimiterService_Acceptable_Rate_Per_Timeout()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientAToken, DataStorageSimulator.AvailableResource.ResourceTwo);
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(1001);
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientAToken, DataStorageSimulator.AvailableResource.ResourceTwo);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsTrue(result);
        }
        /// <summary>
        /// ClientC - both rlRequestsPerInterval and rlRequestsPerTimeout - positive test
        /// </summary>
        [Test]
        public void Test_RateLimiterService_Acceptable_Rate_Per_Both_Timeout_And_Interval()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientCToken, DataStorageSimulator.AvailableResource.ResourceOne);
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(1001);
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientCToken, DataStorageSimulator.AvailableResource.ResourceOne);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsTrue(result);
        }
        /// <summary>
        /// ClientC - both rlRequestsPerInterval and rlRequestsPerTimeout - negative test
        /// </summary>
        [Test]
        public void Test_RateLimiterService_Too_Fast_For_Both_Timeout_And_Interval()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientCToken, DataStorageSimulator.AvailableResource.ResourceThree);
            for (int i = 0; i < 30; i++)
            {
                Thread.Sleep(800);
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientCToken, DataStorageSimulator.AvailableResource.ResourceThree);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Unrated Client Positive test
        /// </summary>
        [Test]
        public void Test_RateLimiterService_Un_Rated_Client()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.UnRatedClientToken, DataStorageSimulator.AvailableResource.ResourceOne);
            for (int i = 0; i < 15; i++)
            {
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.UnRatedClientToken, DataStorageSimulator.AvailableResource.ResourceOne);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsTrue(result);
        }
        /// <summary>
        /// Rated Client with Unrated Resource positive test
        /// </summary>
        [Test]
        public void Test_RateLimiterService_Rated_Client_Unrated_Resource()
        {
            var result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientAToken, DataStorageSimulator.AvailableResource.ResourceThree);
            for (int i = 0; i < 15; i++)
            {
                result = _rateLimiterService.IsRequestAllowed(DataStorageSimulator.Token.ClientAToken, DataStorageSimulator.AvailableResource.ResourceThree);
                if (!result)
                {
                    break;
                }
            }
            Assert.IsTrue(result);
        }

    }
}
