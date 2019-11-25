using RateLimiter.Interfaces;

namespace RateLimiter
{
	public class RateLimitManager : IRateLimitManager
	{
		private readonly IResourceDatabase database;
		private readonly IRulesManager rules;
		private readonly IDateTimeProvider dateProvider;

		public RateLimitManager(
			IResourceDatabase database,
			IRulesManager rules,
			IDateTimeProvider dateProvider)
		{
			this.database = database;
			this.rules = rules;
			this.dateProvider = dateProvider;
		}

		public bool CheckAccess(long resourceId, long userId)
		{
			var date = dateProvider.Now;
			var data = database.GetAccesses(resourceId, userId);
			var result = rules.CheckAccess(data, date);
			if (result)
			{
				database.Add(resourceId, userId, date);
			}
			return result;
		}
	}
}
