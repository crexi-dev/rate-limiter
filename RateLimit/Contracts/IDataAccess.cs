using DataBaseLayer.Models;

namespace RateLimit.Contracts
{
	public interface IDataAccess
	{
		Task<List<Request>> GetRequestByClientIdAsync(string clientId, TimeSpan lastCallPeriod);
		Task<bool> LogRequestByClientAsync(string clientId, DateTime dateTime);
	}
}

