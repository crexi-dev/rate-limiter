using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using RateLimiter.API.Attributes;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Enums;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
		private string clientToken;
		private readonly HttpClient client;
		public RateLimiterTest()
		{
			this.clientToken = Guid.NewGuid().ToString();
			this.client = new HttpClient();
			this.client.BaseAddress = new Uri("https://localhost:44390");
			this.client.DefaultRequestHeaders.Add("x-user-token", clientToken);

		}


        [Test]
		[TestCase(11)]
        [TestCase(34)]
        [TestCase(60)]
        [TestCase(20)]
        [TestCase(100)]
        public void ShouldNotAllowTooManyRequestsBasedOnTime(int amountOfCalls)
        {
			bool shouldLimit = false;
			LimitRateAttribute limitRate = new();
			for (int i = 0; i < amountOfCalls; i++)
			{
				if (limitRate.LimitCalls(clientToken))
				{
					shouldLimit = true;
				}
			}

			Assert.IsTrue(shouldLimit);

        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(9)]
        public void ShouldAllowBasedOnTime(int amountOfCalls)
        {
	        bool shouldLimit = false;
	        LimitRateAttribute limitRate = new LimitRateAttribute();
	        for (int i = 0; i < amountOfCalls; i++)
	        {
		        if (limitRate.LimitCalls(clientToken))
		        {
			        shouldLimit = true;
		        }
	        }

	        Assert.IsFalse(shouldLimit);

        }
		[Test]
        public void ShouldAllowAfterOneMinute()
        {
	        LimitRateAttribute limitRate = new LimitRateAttribute();
	        limitRate.Rule = new[] {LimiterRule.BasedOnTimeSinceLastCall};
	        for (int i = 0; i < 3; i++)
	        {
		        var shouldLimit = limitRate.LimitCalls((clientToken));
		        if (i == 1)
		        {
					Assert.IsTrue(shouldLimit);
					//Not really a good idea, Ideally this should be mocked somehow
					Thread.Sleep(60000);
		        }

		        if (i == 2)
		        {
			        Assert.IsFalse(shouldLimit);

				}
			}

        }
	}
}
