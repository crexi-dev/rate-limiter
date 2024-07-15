using RateLimiter.Interface;
using RateLimiter.Model.Rules;
using RateLimiter.Interface;
using RateLimiter.Model;
using System.Threading.Tasks;
using System;

namespace RateLimiter.Service
{
    public class RateLimitingEngine
    {
        IRequestService _reqsvc;
        public RateLimitingEngine(IRequestService reqsvc)
        {
            _reqsvc = reqsvc;
        }
        public RuleList StartingRuleList { get; set; }= new RuleList();
        public async Task<bool> Evaluate(ResourceRequest req, IClient client)
        {
            var res = await StartingRuleList.Evaluate(client, req);
            client.resourceRequests.Add(req);
            return res;
            
        }
        public async Task<bool> Evaluate(ResourceRequest req, String token)
        {
            var client = _reqsvc.GetClient(token);
            var res = await StartingRuleList.Evaluate(client, req);
            client.resourceRequests.Add(req);
            return res;

        }

    }
}
