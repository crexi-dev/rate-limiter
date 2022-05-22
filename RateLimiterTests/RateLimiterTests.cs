using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiterTests
{
    [TestClass]
    public class RateLimiterTests
    {

  

        [TestMethod]
        public void TokenBucket_Algorithm_Test()
        {
            List<bool> results = new List<bool>();
            //5 tokens per 10 seconds 
            var tokenBucket = new TokenBucketModel(5,10);
            for (int i = 0; i < 10; i++)//make 10 requests
            {
                results.Add(tokenBucket.allowRequest());
            }
            //5 requests should be blocked
            var fails = results.Where(x => x == false).Count();
            Assert.IsTrue(fails == 5);
        }

        [TestMethod]
        public void TokenBucket_Algorithm_Test_with_intervals()
        {
            List<bool> results = new List<bool>();
            //1 tokens per 2 seconds 
            var tokenBucket = new TokenBucketModel(1, 2);
            for (int i = 0; i < 10; i++)//make 10 requests with 1 second intervals
            {
                results.Add(tokenBucket.allowRequest());
                Thread.Sleep(1000);
            }
            //5 requests should be blocked
            var fails = results.Where(x => x == false).Count();
            Assert.IsTrue(fails == 5);
        }
        [TestMethod]
        public void Token_Time_Interval_Algorithm_Test()
        {
            List<bool> results = new List<bool>();
            var tokenInterval = new TokenIntervalModel(2); //2 second interval between requests
            for(int i=0; i<5; i++)
            {
                results.Add(tokenInterval.allowRequest()); //make requests with 2 second intervals
                Thread.Sleep(2000);
            }
            var fails = results.Where(x => x == false).Count();
            Assert.IsTrue(fails == 0); //all requests should pass 

        }

        [TestMethod]
        public void Token_Time_Interval_Algorithm_Test_Fail()
        {
            List<bool> results = new List<bool>();
            var tokenInterval = new TokenIntervalModel(2); //2 second interval between requests
            for (int i = 0; i < 5; i++)
            {
                results.Add(tokenInterval.allowRequest()); //make requests with no intervals
            }
            var fails = results.Where(x => x == false).Count();
            Assert.IsTrue(fails == 4); //all except first request should fail

        }


    }
}
