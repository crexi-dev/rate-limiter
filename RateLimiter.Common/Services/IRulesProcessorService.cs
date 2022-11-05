using System.Collections.Generic;
using RateLimiter.Models;

namespace RateLimiter
{
    public interface IRulesProcessorService
	{
		// აქ მგონი endpoint-ის მაგივრად Key რომ მქონდეს უკეთესი იქნება და სხვა იმპლემენტეციაში შეიძლება სხვა ჭრილი შეინახო
		void AddRequestEvent(string key, RequestEvent requestEvents);
		bool IsValidLimit(List<string> ruleKyes);
	}
}
