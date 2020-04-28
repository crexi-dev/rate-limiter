using RateLimiter.Services;
using RateLimiter.Web.Controllers;
using RateLimiter.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RateLimiter.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RateLimiterByRegionAttribute : ActionFilterAttribute, IRateLimiterAttribute
    {
        ///// <summary>
        ///// Limits the number of requests based on the token region (US/EU).
        ///// </summary>
        ///// <param name="timeSpanInSeconds">number of seconds to calculate access limits.</param>
        ///// <param name="maxRequestsPerTimeSpan">number of maximum requests allowed per timespan.</param>
        ///// <param name="timeElapsedSinceLastCall">number of seconds to disallow subsequent calls. E.g. For timeElapsedSinceLastCall=5 all calls made within 5 seconds from a call will be blocked.</param>
        ///// <param name="isLimitPerAccount">If true limits access per account ID otherwise limits per token.</param>
        public RateLimiterByRegionAttribute(int timeSpanInSeconds, int maxRequestsPerTimeSpan, int timeElapsedSinceLastCall, bool isLimitPerAccount = false)
        {
            TimeElapsedSinceLastCall = timeElapsedSinceLastCall;
            TimeSpanInSeconds = timeSpanInSeconds;
            MaxRequestsPerTimeSpan = maxRequestsPerTimeSpan;
            IsLimitPerAccount = isLimitPerAccount;
        }

        public virtual int TimeElapsedSinceLastCall { get; }

        public virtual int TimeSpanInSeconds { get; }

        public virtual int MaxRequestsPerTimeSpan { get; }

        public virtual bool IsLimitPerAccount { get; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // Headers must be present to simulate the token and account information.
            string token = actionContext.Request.Headers.GetValues("token").First();

            if (token.StartsWith("us-"))
            {
                var attr = new RateLimiterRequestRateAttribute(TimeSpanInSeconds, MaxRequestsPerTimeSpan, IsLimitPerAccount);
                attr.OnActionExecuting(actionContext);
                return;
            }
            if (token.StartsWith("eu-"))
            {
                var attr = new RateLimiterPerTimeSpanAttribute(TimeElapsedSinceLastCall, IsLimitPerAccount);
                attr.OnActionExecuting(actionContext);
                return;
            }

            //any other region is disallowed
            throw new HttpException((int)HttpStatusCode.Forbidden, $"Your token is not allowed to access this resource now. Try again later.");
        }
    }
}
