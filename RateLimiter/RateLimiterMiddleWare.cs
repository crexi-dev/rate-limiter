using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiter.Helpers;
using RateLimiter.Models;
using RateLimiter.Providers;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly RateLimiterConfigModel _rateLimiterConfigModel;
        private  RulesProvider _rulesProvider;
        private ErrorHandler _errorHandler;
        public RateLimiterMiddleWare(RequestDelegate next, IOptions<RateLimiterConfigModel> optionSettings)
        {
            _rateLimiterConfigModel = optionSettings.Value;
            _next = next;
            _errorHandler = new ErrorHandler();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            _rulesProvider = new RulesProvider(httpContext, _rateLimiterConfigModel);


            try
            {
                if (_rulesProvider.evaluateRules())//all the magic happens here
                {
                    await _next(httpContext); //pass request to the pipeline if all rules were met

                }
                else //fail request if any of the rules was violated
                {
                    
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

                    var message = "Too Many Requests";

                    await httpContext.Response.WriteAsync(new ErrorDetailsModel()
                    {
                        StatusCode = httpContext.Response.StatusCode,
                        Message = message
                    }.ToString());
                }
            }
            catch (Exception ex) 
            {
                await _errorHandler.handleExceptionAsync(httpContext, ex);
            }
        }
           
    }

}

