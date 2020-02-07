using RateLimiter.Api.Validators;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace RateLimiter.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class ValidateRateLimitAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Apply rules that limit the timespan between valid requests to the API.
        /// </summary>
        public bool ApplyTimeRules { get; }

        /// <summary>
        /// Apply rules that limit the number of consecutive valid requests to the API per a period of time.
        /// </summary>
        public bool ApplyQuantityRules { get; }

        public ValidateRateLimitAttribute(bool applyTimeRules = false, bool applyQuantityRules = false)
        {
            ApplyTimeRules = applyTimeRules;
            ApplyQuantityRules = applyQuantityRules;
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            RequestValidator.ValidateRequest();
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}