using RateLimiter.Domain.Aggregate;
using RateLimiter.Domain.Contexts;
using RateLimiter.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Application.Interfaces
{
    public interface ITokenBucket : IDisposable
    {
        void EvaluateAndUpdate(EvaluationContext requestContext);

        // TODO : Bucket cleanup for expired tokens
    }
}
