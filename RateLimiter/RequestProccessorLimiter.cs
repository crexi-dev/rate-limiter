using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Rules;

namespace RateLimiter
{
    public class RequestProccessorLimiter : ILimiter
    {
        private const string LimitError = "Limited Restriction";

        private ConcurrentBag<ILimiteRule> rules;
        private IRequestProcessor processor;

        public RequestProccessorLimiter(IRequestProcessor _processor, List<ILimiteRule> _rules)
        {
            rules = new ConcurrentBag<ILimiteRule>();

            foreach (ILimiteRule rule in _rules)
            {
                rules.Add(rule);
            }

            processor = _processor;
        }

        public void AddRule(ILimiteRule rule)
        {
            rules.Add(rule);
        }

        public async Task<Response> DoRequestAsync(Request request)
        {
            bool result = true;

            foreach (ILimiteRule rule in rules)
            {
                if (!rule.CanPassNow(request))
                {
                    result = false;
                    break;
                }
            }

            return result == true
                ? await processor.DoRequestAsync(request)
                : new Response() { IsSuccessful = false, Errors = new List<string>(1) { LimitError } };
        }
    }
}
