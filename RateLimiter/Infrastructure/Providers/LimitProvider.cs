using RateLimiter.Application.Interfaces;
using RateLimiter.Domain.Contexts;
using RateLimiter.Domain.Exceptions;
using RateLimiter.Domain.ValueObjects;

namespace RateLimiter.Infastructure.Providers
{
    public class SlidingWindowWithInterval : ILimitProvider
    {
        public VisitContext Evaluate(EvaluationContext context, VisitContext visit)
        {
            foreach (var rule in context.RuleSet)
            {
                if (rule.Threshold > 1) // Window based
                {
                    var windowEnd = visit.WindowStart + rule.TimeSpan;

                    if (context.RequestContext.TimeStamp <= windowEnd)
                    {
                        if (visit.Counter == rule.Threshold)
                        {
                            return visit;
                        }

                        visit.Counter++;
                    }
                    else
                    {
                        visit.WindowStart = context.RequestContext.TimeStamp;
                        visit.LastAccess = context.RequestContext.TimeStamp;
                        visit.Counter = 1;
                    }
                }
                else // Interval based
                {
                    var sinceLastRequest = context.RequestContext.TimeStamp - visit.LastAccess;
                    if(rule.TimeSpan > sinceLastRequest)
                    {
                        return visit;
                    }

                    visit.LastAccess = context.RequestContext.TimeStamp;
                }
            }

            return visit;
        }
    }
}
