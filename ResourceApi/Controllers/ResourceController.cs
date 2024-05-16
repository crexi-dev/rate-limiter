using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RateLimiter.Interfaces;
using RateLimiter.Limiter;
using RateLimiter.Rules;
using ResourceApi.Helpers;

namespace ResourceApi.Controllers
{    
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly ResourceLimitter resourceLimitter;
        private readonly MixedResourceLimitter mixedResourceLimitter;
        private readonly IAllowRequest allowRequestPerTimespan;
        private readonly IAllowRequest allowSinceLastCall;
        private readonly IAllowRequest allowEURequestLimitRule;
        private readonly IAllowRequest allowUSRequestLimitRule;
        private readonly string NotValidRequest = "Not valid request";
        private readonly string ResourceLimitterIsNull = "ResourceLimitterIsNull";

        public ResourceController()
        {
            resourceLimitter = CreateRateLimiter.ResourceLimitter;
            mixedResourceLimitter = CreateRateLimiter.MixedResourceLimitter;

            allowRequestPerTimespan = CreateRateLimiter.AllowRequestPerTimespan;
            resourceLimitter.AddRule("RequestPerTimespan", allowRequestPerTimespan);

            allowSinceLastCall = CreateRateLimiter.AllowTimespanSinceLastCall;
            resourceLimitter.AddRule("TimeSpanSinceLastCall", allowSinceLastCall);

            allowEURequestLimitRule = CreateRateLimiter.AllowEUReqiestLimit;
            resourceLimitter.AddRule("EUReqiestLimit", allowEURequestLimitRule);

            allowUSRequestLimitRule = CreateRateLimiter.AllowUSRequestLimit;
            resourceLimitter.AddRule("USRequestLimit", allowUSRequestLimitRule);
        }

        [HttpGet, Authorize(Roles = "Admin,Moderator")]
        [Route("GetImgResource")]
        public string GetImageResource()
        {
            var img = "img1.jpg";

            if(resourceLimitter == null)
            {
                return ResourceLimitterIsNull;
            }

            var isValid = resourceLimitter.Validate(img);
            if (!isValid)
            {
               return NotValidRequest;
            }

            return img;
        }
       
        [HttpGet, Authorize(Roles = "Admin")]
        [Route("GetFileResource")]
        public string GetFileResource()
        {
            var fileName = "file.doc";
            if (mixedResourceLimitter == null)
            {
                return ResourceLimitterIsNull;
            }

            var isValid = mixedResourceLimitter.Validate(fileName);

            if (!isValid)
            {
                return NotValidRequest;
            }

            return fileName;
        }        
    }
}
