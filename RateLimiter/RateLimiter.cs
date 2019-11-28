using System;
using System.Collections.Generic;

namespace RateLimiter
{
	public interface IRateLimiter
	{
		bool IsRequestAllowed(string resourceId, string clientId);
	}

	public class RateLimiter : IRateLimiter
	{
		private readonly IStorage storage;
		private readonly IResourceRequestLimit limitRules;

		public RateLimiter(IStorage storage, IResourceRequestLimit limitRules)
		{
			this.storage = storage;
			this.limitRules = limitRules;
		}

		public bool IsRequestAllowed(string resourceId, string clientId)
		{
			var now = DateTime.UtcNow;
			var lastUsage = storage.GetLastUsage(resourceId, clientId);
			storage.SetLastUsage(resourceId, clientId, now);

			if (lastUsage == null)
			{
				return true;
			}

			if (now - lastUsage.Value > limitRules.SinceLastCall)
			{
				return true;
			}

			return false;
		}
	}

	public interface IStorage
	{
		DateTime? GetLastUsage(string resourceId, string clientId);
		void SetLastUsage(string resourceId, string clientId, DateTime lastUsage);
	}

	public class ResourceRestrictionStorage : IStorage
	{
		private readonly Dictionary<string, Dictionary<string, DateTime>> resourceLastRequested = new Dictionary<string, Dictionary<string, DateTime>>();

		public DateTime? GetLastUsage(string resourceId, string clientId)
		{
			if (!resourceLastRequested.ContainsKey(resourceId))
			{
				return null;
			}

			if (!resourceLastRequested[resourceId].ContainsKey(clientId))
			{
				return null;
			}

			return resourceLastRequested[resourceId][clientId];
		}

		public void SetLastUsage(string resourceId, string clientId, DateTime lastUsage)
		{
			if (!resourceLastRequested.ContainsKey(resourceId))
			{
				resourceLastRequested[resourceId] = new Dictionary<string, DateTime>();
			}

			resourceLastRequested[resourceId][clientId] = lastUsage;
		}
	}

	public interface IResourceRequestLimit
	{
		TimeSpan SinceLastCall { get; }
	}

	public class ResourceRequestLimit : IResourceRequestLimit
	{
		public TimeSpan SinceLastCall { get; }

		public ResourceRequestLimit(TimeSpan lastCallRestriction)
		{
			SinceLastCall = lastCallRestriction;
		}
	}



	public interface IRule
	{
		bool IsRuleAccepted(string resourceId, string clientId, DateTime currentRequest);
	}

	public class SinceLastCallRule : IRule
	{
		private readonly IStorage storage;

		public SinceLastCallRule(IStorage storage)
		{
			this.storage = storage;
		}

		public bool IsRuleAccepted(string resourceId, string clientId, DateTime currentRequest)
		{
			throw new NotImplementedException();
		}
	}
}
