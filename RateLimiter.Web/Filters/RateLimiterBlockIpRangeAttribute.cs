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
    public class RateLimiterBlockIpRangeAttribute : ActionFilterAttribute, IRateLimiterAttribute
    {
        ///// <summary>
        ///// Blocks access from IPs between a range of IP addresses.
        ///// </summary>
        ///// <param name="ipRangeStart">Blocked IP range start (inclusive).</param>
        ///// <param name="ipRangeEnd">Blocked IP range end (inclusive).</param>
        public RateLimiterBlockIpRangeAttribute(string ipRangeStart, string ipRangeEnd)
        {
            IpRangeStart = ipRangeStart;
            IpRangeEnd = ipRangeEnd;
        }

        public virtual string IpRangeStart { get; }

        public virtual string IpRangeEnd { get; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            var thisController = ((BaseApiController)actionContext.ControllerContext.Controller);

            IPAddress clientIp = null;
            //In a real scenario would be something like this but mocking HttpContext and HttpContextBase is a pain and I'll mock the IP address using Request Headers.
            //if (actionContext.Request.Properties.ContainsKey("MS_HttpContext"))
            //{
            //    IPAddress.TryParse(((HttpContextBase)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress, out clientIp);
            //}

            IPAddress.TryParse(actionContext.Request.Headers.GetValues("ip").FirstOrDefault(), out clientIp);

            if (clientIp == null)
            {
                throw new Exception("Client IP could not be found");
            }

            if (thisController.AccessService.IsAccessBlockedByIp(IpRangeStart, IpRangeEnd, clientIp)) {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"You are not allowed access to this endpoint.");
            }

        }
    }
}
