using Microsoft.Extensions.Configuration;
using RateLimiter.Exceptions;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    /// <summary>
    /// This rule:
    /// 1. Is applicable for all requests originating in the EU or from an unknown region
    /// 2. Only allows requests to be processed if they are at least a certain distance from the last accepted request.
    /// The time allowed between requests is given by the TimeSinceLastRequestRule_Ms configuration parameter
    /// </summary>
    public class TimeSinceLastRequestRule : RateLimiterBaseRule
    {
        private ICachingService _cachingService;
        private readonly int _msBetweenRequests;

        public TimeSinceLastRequestRule(ICachingService cachingService, IConfiguration configuration)
        {
            _cachingService = cachingService;
            _msBetweenRequests = int.Parse(configuration["TimeSinceLastRequestRule_Ms"]);
        }

        protected override Task<bool> IsApplicableAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            // For illustration purposes, applies to EU-based rules OR Unknown rules 
            return Task.FromResult(request.Location.Region == Region.EU || request.Location.Region == Region.Unknown);
        }

        protected override async Task<bool> IsPermittedAsync(RequestInfo request, CancellationToken cancellationToken)
        {
            try
            {
                await _cachingService.UpdateValue<DateTime>(this.GetCachingKey(request),
                    currentValue => 
                    {
                        if ((request.DateTime - currentValue).Milliseconds < _msBetweenRequests)
                            throw new CacheNotUpdatedException();
                        return request.DateTime;
                    }, 
                    DateTime.MinValue, cancellationToken);
                return true;
            }
            catch (CacheNotUpdatedException)
            {
                return false;
            }
        }

        protected virtual string GetCachingKey(RequestInfo request)
        {
            return typeof(TimeSinceLastRequestRule).Name + "_" + this.GetIdForToken(request.Token); 
        }

    }
}
