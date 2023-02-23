using System;
using NUnit.Framework;
using RateLimiter.Impl;
using RateLimiter.Interfaces;
using System.Collections.Generic;
using System.Threading;
using RateLimiter.Constants;

namespace RateLimiter.Tests
{
	[TestFixture]
	public class RateLimiterTest
	{
		public class NoLimitRulesTest
		{
			private HomeController _controller;

			[SetUp]
			public void TestInit()
			{
				var limitRules = new Dictionary<string, List<ILimiterRule>>();
				_controller = new HomeController(limitRules, new SomeService());
			}

			[Test]
			public void Returns_invalid_response_if_no_limit_rules()
			{
				var clientKey = "unknown_client_key";
				var response = _controller.Index(clientKey);
				
				Assert.IsFalse(response.IsSuccess);
				Assert.IsEmpty(response.Data);
				
				var errorMsg = string.Format(ErrorMessages.NoRulesError, clientKey);
				Assert.AreEqual(response.Error, errorMsg);
			}
		}

		public class TotalRequestCountLimitTest : RateLimiterTest
		{
			private string _clientKey;
			private HomeController _controller;

			[SetUp]
			public void TestInit()
			{
				_clientKey = "test-client-key";
				var limitRules = new Dictionary<string, List<ILimiterRule>>
				{
					{
						_clientKey,
						new List<ILimiterRule>
						{
							new TotalRequestCountLimit { CountLimit = 2 }
						}
					}
				};
				
				_controller = new HomeController(limitRules, new SomeService());
			}

			[Test]
			public void Returns_valid_response()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
			}
			
			[Test]
			public void Returns_invalid_response_if_out_of_limit()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsFalse(_controller.Index(_clientKey).IsSuccess);
			}
		}

		public class RequestsPerTimeLimiterTest : RateLimiterTest
		{
			private string _clientKey;
			private HomeController _controller;

			[SetUp]
			public void TestInit()
			{
				_clientKey = "test-client-key";
				var limitRules = new Dictionary<string, List<ILimiterRule>>
				{
					{
						_clientKey,
						new List<ILimiterRule>
						{
							new RequestsPerTimeLimiter
							{
								RequestLimit = 2,
								TimeRange = TimeSpan.FromSeconds(3)
							}
						}
					}
				};

				_controller = new HomeController(limitRules, new SomeService());
			}

			[Test]
			public void Returns_valid_response()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Thread.Sleep(TimeSpan.FromSeconds(3));
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
			}

			[Test]
			public void Returns_invalid_response_if_out_of_limit()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsFalse(_controller.Index(_clientKey).IsSuccess);
			}
		}

		public class CombinedLimiterTest : RateLimiterTest
		{
			private string _clientKey;
			private HomeController _controller;

			private TotalRequestCountLimit _totalRequestCountLimit;
			private RequestsPerTimeLimiter _requestPerTimeLimiter;

			[SetUp]
			public void TestInit()
			{
				_clientKey = "test-client-key";

				_totalRequestCountLimit = new TotalRequestCountLimit
				{
					CountLimit = 6
				};

				_requestPerTimeLimiter = new RequestsPerTimeLimiter
				{
					RequestLimit = 2,
					TimeRange = TimeSpan.FromSeconds(3)
				};

				var limitRules = new Dictionary<string, List<ILimiterRule>>
				{
					{
						_clientKey,
						new List<ILimiterRule>
						{
							_totalRequestCountLimit,
							_requestPerTimeLimiter
						}
					}
				};

				_controller = new HomeController(limitRules, new SomeService());
			}

			[Test]
			public void Returns_valid_response()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				
				Thread.Sleep(TimeSpan.FromSeconds(3));
				
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				
				Thread.Sleep(TimeSpan.FromSeconds(3));
				
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
			}

			[Test]
			public void Returns_invalid_response_if_out_of_limit_per_time()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				
				Thread.Sleep(TimeSpan.FromSeconds(3));
				
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);

				var response = _controller.Index(_clientKey);
				Assert.IsFalse(response.IsSuccess);
				Assert.IsEmpty(response.Data);
				
				var errorMsg =
					string.Format(
						ErrorMessages.RequestsPerTimeLimiterError,
						_requestPerTimeLimiter.RequestLimit,
						_requestPerTimeLimiter.TimeRange);
				Assert.AreEqual(response.Error, errorMsg);
			}

			[Test]
			public void Returns_invalid_response_if_out_of_limit()
			{
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				
				Thread.Sleep(TimeSpan.FromSeconds(3));
				
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				
				Thread.Sleep(TimeSpan.FromSeconds(3));
				
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				Assert.IsTrue(_controller.Index(_clientKey).IsSuccess);
				
				Thread.Sleep(TimeSpan.FromSeconds(3));

				var response = _controller.Index(_clientKey);
				Assert.IsFalse(response.IsSuccess);
				Assert.IsEmpty(response.Data);
				
				var errorMsg =
					string.Format(
						ErrorMessages.TotalRequestCountLimitError,
						_totalRequestCountLimit.CountLimit);
				Assert.AreEqual(response.Error, errorMsg);
			}
		}
	}
}
