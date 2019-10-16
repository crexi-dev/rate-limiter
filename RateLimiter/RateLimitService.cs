using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimitService
    {
        private IList<RateLimitRule> rules;
        private IList<ClientCallLog> callLogs;
        public RateLimitService()
        {
            rules = new List<RateLimitRule>();
            callLogs = new List<ClientCallLog>();
        }

        public void AddClientRule(RateLimitRule rule)
        {
            rules.Add(rule);
        }

        public bool HasExceededLimit(Guid clientId)
        {
            var clientRule = rules.Where(x => x.ClientId == clientId).FirstOrDefault();

            if(clientRule != null)
            {
                var exceededLimit = false;

                if(clientRule.RequestsPerTimeSpanRule != null)
                {
                    exceededLimit = ExceededRequestsPerTimeSpanRule(clientId, clientRule.RequestsPerTimeSpanRule);
                    if (exceededLimit)
                    {
                        return true;
                    }
                }
                
                if(clientRule.TimeSinceLastCallRule != null)
                {
                    exceededLimit = ExceededTimeSinceLastCallRule(clientId, clientRule.TimeSinceLastCallRule);
                    if (exceededLimit)
                    {
                        return true;
                    }
                }
            }

            AddClientCallLog(new ClientCallLog { ClientId = clientId, CallDateTime = DateTime.UtcNow });

            return false;
        }

        public bool ExceededRequestsPerTimeSpanRule(Guid clientId, RequestsPerTimeSpanRule rule)
        {
            var result = false;

            DateTime startPeriod = DateTime.UtcNow.Subtract(rule.Period);

            int callsInPeriod = callLogs.Where(x => x.ClientId == clientId && x.CallDateTime > startPeriod).Count();

            if(callsInPeriod >= rule.MaxCallsAllowed)
            {
                return true;
            }

            return result;
        }

        public bool ExceededTimeSinceLastCallRule(Guid clientId, TimeSinceLastCallRule rule)
        {
            var result = false;

            var lastCallLog = callLogs.Where(x => x.ClientId == clientId).OrderByDescending(x => x.CallDateTime).FirstOrDefault();

            if(lastCallLog != null)
            {
                DateTime now = DateTime.UtcNow;
                DateTime lastCall = lastCallLog.CallDateTime;

                if(lastCall.Add(rule.Period) > now)
                {
                    return true;
                }
            }

            return result;
        }

        private void AddClientCallLog(ClientCallLog log)
        {
            callLogs.Add(log);
        }
    }
}
