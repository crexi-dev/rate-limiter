using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace RateLimiter.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RuleALimitAttribute : ActionFilterAttribute
    {
        public class RuleAUserInfo
        {
            public DateTime StartedDate { get; set; }
            public int RequestsCurrentCount { get; set; }
        }

        private static readonly TimeSpan LimitTime = TimeSpan.FromSeconds(100);
        private static readonly int RequestsMaxCount = 5;
        public static Dictionary<int, RuleAUserInfo> UserLimits = new(); 

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(filterContext == null)
            {
                return;
            }

            var accessToken = filterContext.HttpContext.Request.Headers["AccessToken"];

            if(String.IsNullOrEmpty(accessToken))
            {
                filterContext.Result = new ContentResult
                {
                    Content = "Unauthorized"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);

            var strUserId = jwt?.Claims?.FirstOrDefault(_ => _.Type == "id")?.Value;

            if (String.IsNullOrEmpty(strUserId))
            {
                filterContext.Result = new ContentResult
                {
                    Content = "Unauthorized"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var success = int.TryParse(strUserId, out int userId);
            if(!success)
            {
                filterContext.Result = new ContentResult
                {
                    Content = "Unauthorized"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var isThereAccess = CheckLimits(userId);
            if(!isThereAccess)
            {
                filterContext.Result = new ContentResult
                {
                    Content = $"You are allowed to make requests only {RequestsMaxCount} times in {LimitTime.Seconds} seconds"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }
        }

        public bool CheckLimits(int userId)
        {
            if (UserLimits.ContainsKey(userId))
            {
                var userLimit = UserLimits[userId];
                if (userLimit.RequestsCurrentCount < RequestsMaxCount)
                {
                    userLimit.RequestsCurrentCount++;
                    return true;
                }
                if (userLimit.RequestsCurrentCount == RequestsMaxCount)
                {
                    if (userLimit.StartedDate.AddSeconds(LimitTime.Seconds) < DateTime.UtcNow)
                    {
                        userLimit.StartedDate = DateTime.UtcNow;
                        userLimit.RequestsCurrentCount = 1;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            else
            {
                UserLimits[userId] = new RuleAUserInfo()
                {
                    RequestsCurrentCount = 1,
                    StartedDate = DateTime.UtcNow
                };
                return true;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RuleBLimitAttribute : ActionFilterAttribute
    {
        private static readonly TimeSpan LimitTime = TimeSpan.FromSeconds(20);

        //value is the DateTime of the last request
        public static Dictionary<int, DateTime> UserLimits = new();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                return;
            }

            var accessToken = filterContext.HttpContext.Request.Headers["AccessToken"];

            if (String.IsNullOrEmpty(accessToken))
            {
                filterContext.Result = new ContentResult
                {
                    Content = "Unauthorized"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);

            var strUserId = jwt?.Claims?.FirstOrDefault(_ => _.Type == "id")?.Value;

            if (String.IsNullOrEmpty(strUserId))
            {
                filterContext.Result = new ContentResult
                {
                    Content = "Unauthorized"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var success = int.TryParse(strUserId, out int userId);
            if (!success)
            {
                filterContext.Result = new ContentResult
                {
                    Content = "Unauthorized"
                };
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var thereIsAccess = CheckLimits(userId);
            if(!thereIsAccess)
            {
                filterContext.Result = new ContentResult
                {
                    Content = $"You are allowed to make requests per after {UserLimits[userId] + LimitTime} UTC"
                };

                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }
        }

        public bool CheckLimits(int userId)
        {
            if (UserLimits.ContainsKey(userId))
            {
                var lastRequestDateTime = UserLimits[userId];
                UserLimits[userId] = DateTime.UtcNow;

                if (lastRequestDateTime.AddSeconds(LimitTime.Seconds) < DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                UserLimits[userId] = DateTime.UtcNow;
                return true;
            }
        }
    }
}
