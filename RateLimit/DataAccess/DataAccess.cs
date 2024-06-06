using DataBaseLayer;
using DataBaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using RateLimit.Contracts;

namespace RateLimit.DataAccess
{
	public class DataAccess : IDataAccess
	{
		private readonly DataBaseContext _context;

		public DataAccess(DataBaseContext context)
		{
			_context = context;
		}

		public async Task<List<Request>> GetRequestByClientIdAsync(string clientId, TimeSpan lastCallPeriod)
		{
			var startDate = new DateTimeOffset(DateTime.Now.Subtract(lastCallPeriod));

			var result = await _context.Requests.AsNoTracking().Where(x => x.ClientId == clientId && x.RequestDatetime >= startDate).ToListAsync();
			await LogRequestByClientAsync(clientId, DateTime.Now);
			return result;
		}

		public async Task<bool> LogRequestByClientAsync(string clientId, DateTime requestDateTime)
		{
			var req = new Request() { ClientId = clientId, RequestDatetime = new DateTimeOffset(requestDateTime) };

			_context.Requests.Add(req);

			return await _context.SaveChangesAsync() == 1;
		}
	}
}
