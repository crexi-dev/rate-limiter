using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Moq;
using NUnit.Framework;
using RateLimiter.ClientStatistics;
using RateLimiter.Interfaces;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	[TestCase("/tes/","get", "eu")]
	[TestCase("/tes1/","post", "eu")]
	[TestCase("/2tes/","put", "us")]
	[TestCase("/wtes/","patch", "uk")]
	
	public void SimpleCheckMatcherLogic(string url, string httpMethod, string region)
	{
		var matcher = MatchersFactory.CreateMatchers(new RequestCountConfiguration()
		{
			Url = "*",
			HttpMethod = "*",
			Regions = "*"

		});

		Assert.IsTrue(matcher.IsMatch(new RequestInformation(url,
			new ClientIdentity("0.0.0.0")
			{
				Region = new Region()
				{
					RegionCode = region
				}
			}, httpMethod)));
	}
	
	
	[TestCase("/test/","get", "eu", true)]
	[TestCase("/tes1/","post", "eu", false)]
	[TestCase("/2tes/","put", "us", false)]
	[TestCase("/wtes/","patch", "uk", false)]
	
	public void SimpleUrlCheckMatcherLogic(string url, string httpMethod, string region, bool expected)
	{
		var matcher = MatchersFactory.CreateMatchers(new RequestCountConfiguration()
		{
			Url = "/test/",
			HttpMethod = "*",
			Regions = "*"

		});

		Assert.AreEqual(expected, matcher.IsMatch(new RequestInformation(url,
			new ClientIdentity("0.0.0.0")
			{
				Region = new Region()
				{
					RegionCode = region
				}
			}, httpMethod)));
	}
	
	[TestCase("/test/","get", "eu", false)]
	[TestCase("/tes1/","post", "eu", false)]
	[TestCase("/2tes/","put", "us", true)]
	[TestCase("/wtes/","patch", "uk", false)]
	
	public void SimpleHttpMethodCheckMatcherLogic(string url, string httpMethod, string region, bool expected)
	{
		var matcher = MatchersFactory.CreateMatchers(new RequestCountConfiguration()
		{
			Url = "*",
			HttpMethod = "put",
			Regions = "*"

		});

		Assert.AreEqual(expected, matcher.IsMatch(new RequestInformation(url,
			new ClientIdentity("0.0.0.0")
			{
				Region = new Region()
				{
					RegionCode = region
				}
			}, httpMethod)));
	}
	
	[TestCase("/test/","get", "eu", false)]
	[TestCase("/tes1/","post", "eu", false)]
	[TestCase("/2tes/","put", "us", true)]
	[TestCase("/wtes/","patch", "uk", false)]
	
	public void SimpleRegionCheckMatcherLogic(string url, string httpMethod, string region, bool expected)
	{
		var matcher = MatchersFactory.CreateMatchers(new RequestCountConfiguration()
		{
			Url = "*",
			HttpMethod = "put",
			Regions = "*"

		});

		Assert.AreEqual(expected, matcher.IsMatch(new RequestInformation(url,
			new ClientIdentity("0.0.0.0")
			{
				Region = new Region()
				{
					RegionCode = region
				}
			}, httpMethod)));
	}

	private ITimeProvider GetProviderWithNow(DateTime now)
	{
		var mock = new Mock<ITimeProvider>();
		mock.SetupGet(c => c.Now).Returns(now);
		return mock.Object;
	}

	private ClientIdentity CreateClient(string ip = "0.0.0.0", string region = "us", string token = "34")
	{
		return new ClientIdentity(ip)
		{
			Region = new Region()
			{
				RegionCode = region
			},
			Token = token
		};
	}
	
	[Test]
	public void SimpleRateLimitLogic()
	{
		var option = new RequestCountConfiguration()
		{
			Url = "*",
			HttpMethod = "*",
			Regions = "*",
			Count = 1,
			Duration = new TimeSpan(0, 0, 1, 0)
		};
		var client = CreateClient();
		var request = new RequestInformation("/test", client, "Get");
		var matcher = MatchersFactory.CreateMatchers(option);
		
		var usages = new LinkedList<DateTime>();
		usages.AddFirst(new DateTime(2023, 3, 3, 0, 0, 1));
		usages.AddFirst(new DateTime(2023, 3, 3, 0, 1, 0));
		var statStorage = new StatisticsStorage();
		var stat = new PeriodTotalRequestRateLimitStatistics(new Dictionary<IBucketIdentifier, LinkedList<DateTime>>()
		{
			{matcher.GetRequestId(request), usages}
		});
		statStorage.AddStorage(stat);
		
		var factory = new HandlerFactory(new RuleFactory(), statStorage, GetProviderWithNow(new DateTime(2023, 3, 3, 0, 1, 2)));
		var handler = factory.CreateHandlers(new List<RateLimitConfiguration>()
		{
			new RequestCountConfiguration()
			{
				Url = "*",
				HttpMethod = "*",
				Regions = "*",
				Count = 1,
				Duration = new TimeSpan(0,0,1,0)
			}
		});
		
		Assert.AreEqual(1, handler.Count);
		
		Assert.DoesNotThrow( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});
		
		Assert.Throws<HttpRequestException>( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});
	}
	
	[Test]
	public void SimpleRateLimitLogicTwoClient()
	{
		var option = new RequestCountConfiguration()
		{
			Url = "*",
			HttpMethod = "*",
			Regions = "*",
			Count = 1,
			Duration = new TimeSpan(0, 0, 1, 0)
		};
		var client = CreateClient();
		var client2 = CreateClient(token:"15");
		var request = new RequestInformation("/test", client, "Get");
		var request2 = new RequestInformation("/test", client2, "Get");
		var matcher = MatchersFactory.CreateMatchers(option);
		
		var usages = new LinkedList<DateTime>();
		usages.AddFirst(new DateTime(2023, 3, 3, 0, 0, 1));
		usages.AddFirst(new DateTime(2023, 3, 3, 0, 1, 0));
		var usages2 =  new LinkedList<DateTime>();
		usages2.AddFirst(new DateTime(2023, 3, 3, 0, 0, 1));
		usages2.AddFirst(new DateTime(2023, 3, 3, 0, 1, 0));
		var statStorage = new StatisticsStorage();
		var stat = new PeriodTotalRequestRateLimitStatistics(new Dictionary<IBucketIdentifier, LinkedList<DateTime>>()
		{
			{matcher.GetRequestId(request), usages},
			{matcher.GetRequestId(request2), usages2}
		});
		statStorage.AddStorage(stat);
		
		var factory = new HandlerFactory(new RuleFactory(), statStorage, GetProviderWithNow(new DateTime(2023, 3, 3, 0, 1, 2)));
		var handler = factory.CreateHandlers(new List<RateLimitConfiguration>()
		{
			new RequestCountConfiguration()
			{
				Url = "*",
				HttpMethod = "*",
				Regions = "*",
				Count = 1,
				Duration = new TimeSpan(0,0,1,0)
			}
		});
		
		Assert.AreEqual(1, handler.Count);
		
		Assert.DoesNotThrow( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});

		Assert.Throws<HttpRequestException>( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});
		Assert.DoesNotThrow( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client2, "Get"));
		});
		
		Assert.Throws<HttpRequestException>( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});
		Assert.Throws<HttpRequestException>( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client2, "Get"));
		});
	}
	
	[Test]
	public void SimpleLastusageRateLimitLogic()
	{
		var option = new LastUsageConfiguration()
		{
			Url = "*",
			HttpMethod = "*",
			Regions = "*",
			DelayTime = new TimeSpan(0, 0, 1, 0)
		};
		var client = CreateClient();
		var request = new RequestInformation("/test", client, "Get");
		var matcher = MatchersFactory.CreateMatchers(option);
		
		var statStorage = new StatisticsStorage();
		var stat = new LastUsageRateLimitStatistics(new Dictionary<IBucketIdentifier, DateTime>()
		{
			{matcher.GetRequestId(request), new DateTime(2023, 3, 3, 0, 0, 1)}
		});
		statStorage.AddStorage(stat);
		
		var factory = new HandlerFactory(new RuleFactory(), statStorage, GetProviderWithNow(new DateTime(2023, 3, 3, 0, 1, 2)));
		var handler = factory.CreateHandlers(new List<RateLimitConfiguration>()
		{
			option
		});
		
		Assert.AreEqual(1, handler.Count);
		
		Assert.DoesNotThrow( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});
		
		Assert.Throws<HttpRequestException>( () =>
		{
			handler.First().HandleRequestForUser(new RequestInformation("/test", client, "Get"));
		});
	}
}