using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Settings;
using Microsoft.Extensions.Options;
using RateLimiter.RateLimitRules;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class WorkerService : IHostedService
    {
        private readonly IRulesConfigService _rulesConfigService;
        private readonly RateLimiterRules _initRules;

        public WorkerService(IRulesConfigService rulesConfigService, IOptions<RateLimiterRules> options)
        {
            _rulesConfigService = rulesConfigService;
            _initRules = options.Value;
        }

       
        public Task StartAsync(CancellationToken cancellationToken)
        {

            foreach (var rule in _initRules.RulesSettings)
            {
                var rulePerRequests = rule.Rules.Where(w=>w.RulePerRequest != null).Select(s => s.RulePerRequest).ToList();
                var rulePerTimeSpans = rule.Rules.Where(w => w.RulePerTimeSpan != null).Select(s => s.RulePerTimeSpan).ToList();

                List<IRateLimitRule> rules = new List<IRateLimitRule>();
               
                if (rulePerRequests != null && rulePerRequests.Count > 0)
                {
                    rules.AddRange(rulePerRequests);
                }
                
                if (rulePerTimeSpans != null && rulePerTimeSpans.Count > 0)
                {
                    rules.AddRange(rulePerTimeSpans);
                }
                

                _rulesConfigService.SetRules(rule.Key, rules);


            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
