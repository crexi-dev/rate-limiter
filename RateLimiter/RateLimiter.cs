using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
	public class RateLimiter
	{
		/// <summary>
		/// Default Rules
		/// </summary>
		private readonly ResourceRules _defaultRules;

		/// <summary>
		/// Resource Rules by Resource ID
		/// </summary>
		private readonly IDictionary<string, ResourceRules> _resourcesRules;

		/// <summary>
		/// Requests by User Token and Resource ID
		/// </summary>
		private readonly IDictionary<string, IDictionary<string, IList<DateTime>>> _requests;

		public RateLimiter(ResourceRules defaultRules, IDictionary<string, ResourceRules> resourcesRules)
		{
			_defaultRules = defaultRules;
			_resourcesRules = resourcesRules;

			_requests = new Dictionary<string, IDictionary<string, IList<DateTime>>>();
		}

		/// <summary>
		/// Check if resource allowed
		/// </summary>
		/// <param name="userToken">User Token</param>
		/// <param name="resourceId">Resource ID</param>
		/// <returns>Is allowed</returns>
		public bool AskForResource(string userToken, string resourceId)
		{
			var isAllowed = true; // Result

			var resourceRules = GetResourceRules(resourceId);
			var resourceRequests = GetResourceRequests(userToken, resourceId);
			IList<DateTime> updatedresourceRequests = null;

			var dateNow = DateTime.Now;


			if (resourceRules.RequestPerTimespan > 0 && resourceRules.Timespan > 0) // Max requests per timespan is set
			{
				var startDate = dateNow.AddMilliseconds(-resourceRules.Timespan);
				var lastRequests = resourceRequests.Where(o => o >= startDate).ToList();
				var requestsCount = lastRequests.Count;

				if (requestsCount >= resourceRules.RequestPerTimespan)
				{
					isAllowed = false;
				}

				updatedresourceRequests = lastRequests;
				updatedresourceRequests.Add(dateNow);
			}


			if (isAllowed && resourceRules.MinCallTimespan > 0) // Min time between requests is set
			{
				var lastRequestTime = resourceRequests.LastOrDefault();

				var period =  dateNow - lastRequestTime;

				if (period.TotalMilliseconds < resourceRules.MinCallTimespan)
				{
					isAllowed = false;
				}

				if (updatedresourceRequests == null)
				{
					updatedresourceRequests = new List<DateTime>{ dateNow }; // Only last request need
				}
			}


			UpdateResourceRequests(userToken, resourceId, updatedresourceRequests ?? new List<DateTime>());


			return isAllowed;
		}

		private ResourceRules GetResourceRules(string resourceId)
		{
			if (_resourcesRules.ContainsKey(resourceId))
			{
				return _resourcesRules[resourceId];
			}

			return _defaultRules;
		}

		private IList<DateTime> GetResourceRequests(string userToken, string resourceId)
		{
			IList<DateTime> result = null;

			if (_requests.ContainsKey(userToken))
			{
				var userRequests = _requests[userToken];

				if (userRequests.ContainsKey(resourceId))
				{
					result = userRequests[resourceId];
				}
			}

			return result ?? new List<DateTime>();
		}

		private void UpdateResourceRequests(string userToken, string resourceId, IList<DateTime> newRequests)
		{
			IDictionary<string, IList<DateTime>> userRequests;

			if (_requests.ContainsKey(userToken))
			{
				userRequests = _requests[userToken];
			}
			else
			{
				userRequests = new Dictionary<string, IList<DateTime>>();
				_requests.Add(userToken, userRequests);
			}

			if (userRequests.ContainsKey(resourceId))
			{
				userRequests[resourceId] = newRequests;
			}
			else
			{
				userRequests.Add(resourceId, newRequests);
			}
		}
	}
}
