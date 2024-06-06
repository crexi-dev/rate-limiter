using RateLimit.Contracts;

namespace RateLimit.Services
{
	public class LimitService : ILimitService
	{
		private readonly IDataAccess _dataAccess;

		public LimitService(IDataAccess dataAccess)
		{
			_dataAccess = dataAccess;
		}

		public async Task<bool> IsAccessAllowedAsync(int limit, TimeSpan lastCallPeriod, string clientId)
		{
			var clientsRequests = await _dataAccess.GetRequestByClientIdAsync(clientId, lastCallPeriod);

			return limit > clientsRequests.Count();
		}
	}
}
