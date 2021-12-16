using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RateLimiter.Api.Enums;

namespace RateLimiter.API.Attributes
{
	public class LimitRateAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// Rules to apply rate limiting, can be used in combination
		/// </summary>
		public LimiterRule[] Rule = new[] { LimiterRule.BasedOnTime };

		/// <summary>
		/// Request per minute for BasedOnTime rule
		/// </summary>
		public int RequestsPerMinute = 10;

		/// <summary>
		/// Period of how how often can you make a call in seconds, only used with BasedOnTimeSinceLastCall rule
		/// </summary>
		public int Period = 10;

		private TimeSpan timeSpan => TimeSpan.FromSeconds(Period);

		private ConcurrentDictionary<string, ClientCallModel> CallerData = new();
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var clientToken = context.HttpContext.Request.Headers["x-user-token"].ToString();
			if (LimitCalls(clientToken))
			{
				context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
				return;
			}

			base.OnActionExecuting(context);
		}
		public bool LimitCalls(string clientToken)
		{
			bool shouldLimit = false;

			CallerData.TryGetValue(clientToken, out var lastRequest);
			if (lastRequest != null)
			{
				if (Rule.Contains(LimiterRule.BasedOnTime)
				    && DateTime.Now < lastRequest.RequestDate.AddMinutes(1)
				    && lastRequest.NumberOfRequests >= RequestsPerMinute
				    || Rule.Contains(LimiterRule.BasedOnTimeSinceLastCall)
				    && DateTime.Now < lastRequest.RequestDate.Add(this.timeSpan))
				{
					shouldLimit = true;
				}

				if (DateTime.Now > lastRequest.RequestDate.AddMinutes(1))
				{
					CallerData.TryUpdate(clientToken, new ClientCallModel(DateTime.Now, 1), lastRequest);
				}

			}

			CallerData.AddOrUpdate(clientToken, new ClientCallModel(DateTime.Now, 1), (token, currentModel) =>
			{
				currentModel.NumberOfRequests += 1;
				currentModel.RequestDate = DateTime.Now;
				return currentModel;
			});

			return shouldLimit;
		}
	}

	internal class ClientCallModel
	{
		public ClientCallModel(DateTime requestDate, int numberOfRequests)
		{
			this.RequestDate = requestDate;
			this.NumberOfRequests = numberOfRequests;
		}

		public DateTime RequestDate { get; set; }
		public int NumberOfRequests { get; set; }
	}
}
