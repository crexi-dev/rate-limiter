using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace RateLimiter.Rules
{
    public class RequestsPerTimespan : ILimitRuleInMemory<RequestsPerTimespanRecord>
    {
        public TimeSpan Time { get; set; }
        public int Count { get; set; }

        public List<RequestsPerTimespanRecord> Cache { get; set; } = new();

        public RequestsPerTimespan(TimeSpan time, int count)
        {
            Time = time;
            Count = count;
        }

        private string GetToken(HttpContext context)
        {
            return context.Request.Headers["token"];
        }

        public bool ExecuteRule(HttpContext context)
        {
            var currentClientRequests = Cache
                .Where(x => x.Token == GetToken(context) &&
                            x.Date > DateTime.UtcNow.AddMilliseconds(-Time.TotalMilliseconds))
                .ToList();

            var ordered = currentClientRequests.OrderBy(x => x.Date).ToList();

            var first = ordered.FirstOrDefault();
            var current = ordered.FirstOrDefault(x => x.RequestId == context.TraceIdentifier);

            if (ordered.Count() > Count)
            {
                return false;
            }

            return true;
        }

        public void CollectRequests(HttpContext context)
        {
            var request = Cache.FirstOrDefault(x => x.RequestId == context.TraceIdentifier);

            if (request == null)
            {
                Cache.Add(new RequestsPerTimespanRecord
                {
                    RequestId = context.TraceIdentifier,
                    Token = GetToken(context),
                    Date = DateTime.UtcNow
                });
            }
        }
    }

    public class RequestsPerTimespanRecord
    {
        public string RequestId { get; set; }
        public string Token { get; set; }
        public DateTime Date { get; set; }
    }
}
