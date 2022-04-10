using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace RateLimiter.Common
{
    public class ApiClient : IApiClient
    {
        private IHttpContextAccessor httpContextAccessor;
        public ApiClient(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Guid ClientId
        {
            get
            {
                return Guid.Parse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "ClientId").Value);
            }
        }

        public string Region
        {
            get
            {
                return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Region").Value;
            }
        }
    }
}
