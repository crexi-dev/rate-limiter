using RateLimiter.Application.Interfaces;
using RateLimiter.Domain.Aggregate;
using RateLimiter.Domain.Contexts;
using RateLimiter.Domain.Exceptions;
using RateLimiter.Domain.ValueObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Infastructure.Persistance
{
    public class InMemoryTokenBucket : ITokenBucket
    {
        private readonly ConcurrentDictionary<string, Lazy<VisitContext>> _bucket;
        private readonly ILimitProvider _limitProvider;

        public InMemoryTokenBucket(ILimitProvider limitProvider)
        {
            _limitProvider = limitProvider;
            _bucket = new ConcurrentDictionary<string, Lazy<VisitContext>>();
        }

        public int count => _bucket.FirstOrDefault().Value.Value.Counter;

        public void EvaluateAndUpdate(EvaluationContext context)
        {
            var key = $"{context.RequestContext.ClientContext.Token}-{context.RequestContext.Resource}";
            var failed = false;

            _ = _bucket.AddOrUpdate<EvaluationContext>(key,
                (k, c) => new Lazy<VisitContext>(() => 
                { 
                    return new VisitContext
                    {
                        Counter = 1,
                        LastAccess = context.RequestContext.TimeStamp,
                        WindowStart = context.RequestContext.TimeStamp
                    }; 
                }),
                (k,v,c) => new Lazy<VisitContext>(() =>
                {
                    var prev = v.Value.Clone();
                    var visit = _limitProvider.Evaluate(c, v.Value);
                    failed = prev.Equals(visit);   
                    return visit;
                }), context).Value;

            if (failed)
            {
                throw new LimitExceededException(context.RequestContext.ClientContext.Token);
            }
        }

        public void Dispose()
        {
            _bucket.Clear();
        }
    }
}
