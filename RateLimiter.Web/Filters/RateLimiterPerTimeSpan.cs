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
    public class RateLimiterPerTimeSpanAttribute : ActionFilterAttribute, IRateLimiterAttribute
    {
        ///// <summary>
        ///// Limits the number of requests per timespan. Requests before the time configured elapses will be forbidden.
        ///// </summary>
        ///// <param name="timeSpanInSeconds">number of seconds to calculate access limits.</param>
        ///// <param name="isLimitPerAccount">If true limits access per account ID otherwise limits per token.</param>
        public RateLimiterPerTimeSpanAttribute(int timeSpanInSeconds, bool isLimitPerAccount = false)
        {
            TimeSpanInSeconds = timeSpanInSeconds;
            IsLimitPerAccount = isLimitPerAccount;
        }

        public virtual int TimeSpanInSeconds { get; }

        public virtual bool IsLimitPerAccount { get; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            var thisController = ((BaseApiController)actionContext.ControllerContext.Controller);
            // Headers must be present to simulate the token and account information.
            int account = int.Parse(actionContext.Request.Headers.GetValues("account").First() ?? "0");
            string token = actionContext.Request.Headers.GetValues("token").First();

            if (!thisController.AccessService.IsAccessAllowedPerTimeSpan(account, token, TimeSpanInSeconds, IsLimitPerAccount))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"Your { (IsLimitPerAccount ? "account" : "token")  } is not allowed to access this resource now. Try again later.");
            }

        }
    }

}
