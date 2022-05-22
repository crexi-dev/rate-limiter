using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Helpers
{
    public  class ErrorHandler
    {
      
        public async Task handleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = exception.Message;

            await context.Response.WriteAsync(new ErrorDetailsModel()
            {
                StatusCode = context.Response.StatusCode,
                Message ="RateLimiter Error: "+ message
            }.ToString());
        }
    }
}
