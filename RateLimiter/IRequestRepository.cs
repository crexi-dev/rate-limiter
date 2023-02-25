using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public interface IRequestRepository
    {
        void AddRequest(string username, string resource, DateTime requestTime, Dictionary<string, string> claims);
        RequestLog? GetLastRequest(string username, string resource);
        List<RequestLog> GetRequestsByTimeSpan(string username, string resource, TimeSpan interval);
    }
}
