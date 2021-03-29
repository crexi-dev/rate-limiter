using RateLimiter.Application.Interfaces;
using RateLimiter.Domain.Contexts;
using RateLimiter.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class Api
    {
        public static string Accepted = "ACCEPTED";
        public static string Rejected = "REJECTED";
        public static string Error = "ERROR";

        private readonly IIdentityService _identityService;
        private readonly IDateTime _dateTimeService;
        private readonly IRequestLimitService _limitHandler;


        public Api(IIdentityService identityService, IDateTime dateTimeService, IRequestLimitService limitHandler)
        {
            _identityService = identityService;
            _dateTimeService = dateTimeService;
            _limitHandler = limitHandler;
        }

        /// <summary>
        /// Dummy api method
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string ReceiveRequest(string resource, string token)
        {
            try
            {
                var requestContext = new RequestContext
                {
                    ClientContext = _identityService.VerifyToken(token),
                    Resource = resource,
                    TimeStamp = _dateTimeService.Timestamp
                };

                _limitHandler.Evaluate(requestContext);

                return Accepted;
            }
            catch(LimitExceededException)
            {
                return Rejected;
            }
            catch(Exception)
            {
                return Error;
            }
        }
    }
}
