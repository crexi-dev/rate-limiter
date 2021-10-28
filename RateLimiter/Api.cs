
using RateLimiter.ApiRule.Factory;
using RateLimiter.Model.Enum;
using RateLimiter.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RateLimiter
{
    public class Api
    {
        private readonly RequestValidator _requestValidator = new RequestValidator(new RateLimitedFactory());



        public Api()
        {
            

        }

        public string ResourceA(string token)
        {
            return HandleRequest(token, ResourceEnum.resourcea);
        }

        public string ResourceB(string token)
        {
            return HandleRequest(token, ResourceEnum.resourceb);
        }

        public string ResourceC(string token)
        {
            return HandleRequest(token, ResourceEnum.resourcec);
        }

        private string HandleRequest(string token, ResourceEnum resourceName)
        {
            if (_requestValidator.ValidateRequest(token, resourceName))
                return "Succeeded";

            return "Failed";
        }
    }
}
