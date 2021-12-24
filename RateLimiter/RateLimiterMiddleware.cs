using RateLimiter.LimitRules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterMiddleware
    {
        private readonly Dictionary<string, IEnumerable<ILimitRule>> limitterRules;               //key - source name
        private readonly static Dictionary<string, List<SimpleHttpContext>> lastRequests = new(); //key - source name

        private readonly SimpleRequestDelegate _next;

        public RateLimiterMiddleware(SimpleRequestDelegate next, Dictionary<string, IEnumerable<ILimitRule>> limitterRules)
        {
            this.limitterRules = limitterRules;
            _next = next;
        }

        public void Invoke(SimpleHttpContext context) 
        {
            if (CheckAccess(context))
            {
                SaveRequest(context);
                _next(context);
            }
            else 
            {
                Console.WriteLine("Denied");
            }
        }

        private bool CheckAccess(SimpleHttpContext context)
        {
            if (limitterRules.ContainsKey(context.SourceName))
            {
                lastRequests.TryGetValue(context.SourceName, out var reuestsList);
                foreach (var rule in limitterRules[context.SourceName])
                {
                    if (!rule.Check(context, reuestsList)) return false;
                }
            }
            return true;
        }

        private void SaveRequest(SimpleHttpContext context) 
        {
            if (lastRequests.ContainsKey(context.SourceName))
            {
                lastRequests[context.SourceName].Add(context);
            }
            else 
            {
                lastRequests.Add(context.SourceName, new List<SimpleHttpContext> { context });
            }
        }
    }


}
