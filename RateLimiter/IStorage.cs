using System;

namespace RateLimiter
{
	public interface IStorage
	{
		void UpdateLatestRequest(string resource, IRequest request);

		void AddRequestAndPurge(string resource, TimeSpan interval, IRequest request, int requestLimit);

		IRequest GetLastTimeSpanRequestByResource(string resource);

		int GetXRequestsPerTimeSpanRequestCountByResource(string resource, IRequest request, TimeSpan interval);
	}
}
