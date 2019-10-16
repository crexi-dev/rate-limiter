using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    //Pretend this is an api
    public class SampleApi
    {
        private RateLimitService rateService;

        public SampleApi()
        {
            //should use dependency injection
            rateService = new RateLimitService();
        }

        public SampleApi(RateLimitService rateService)
        {
            this.rateService = rateService;
        }

        public ApiResponse SampleApiMethod(Guid clientId)
        {
            ApiResponse response = new ApiResponse();

            if(rateService.HasExceededLimit(clientId))
            {
                response.Message = "Rate Limit Reached";
                response.RateLimitError = true;
                return response;
            }

            response.Message = "Sample Success Response";
            return response;
        }
    }
}
