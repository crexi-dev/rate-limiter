using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter
{
    public class RequestLog
    {
        public Dictionary<string, string> Claims { get; set; }
        public DateTime RequestTime { get; set; }
    }
    public class InMemoryRequestRepository : IRequestRepository
    {
        private readonly Dictionary<string, List<RequestLog>> _requestsByUserAndResource = new Dictionary<string, List<RequestLog>>();

        public void AddRequest(string username, string resource, DateTime requestTime, Dictionary<string, string> claims)
        {
            var key = GetKey(username, resource);
            if (!_requestsByUserAndResource.TryGetValue(key, out var requestLogs))
            {
                requestLogs = new List<RequestLog> {
                    new RequestLog{RequestTime = requestTime, Claims = claims }
                };
                _requestsByUserAndResource[key] = requestLogs;
            }
            requestLogs.Add(new RequestLog { RequestTime = requestTime, Claims = claims });
        }

        public List<RequestLog> GetRequestsByTimeSpan(string username, string resource, TimeSpan interval)
        {
            var key = GetKey(username, resource);
            if (!_requestsByUserAndResource.TryGetValue(key, out var requestTimes))
            {
                return new List<RequestLog>();
            }
            var now = DateTime.UtcNow;
            var earliestAllowedTime = now - interval;
            return requestTimes.Where(t => t.RequestTime >= earliestAllowedTime).ToList();
        }
        public RequestLog? GetLastRequest(string username, string resource)
        {
            var key = GetKey(username, resource);
            if (!_requestsByUserAndResource.TryGetValue(key, out var requestTimes))
            {
                return null;
            }
            return requestTimes.OrderBy(x => x.RequestTime)
                .LastOrDefault();
        }
        private static string GetKey(string username, string resource)
        {
            return $"{username}::{resource}";
        }
    }
}
