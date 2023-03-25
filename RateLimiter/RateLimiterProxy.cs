using RateLimiter.Handlers;
using RateLimiter.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiterProxy : ResourceProxy
    {


        List<ILimiterHandler> handlers = new List<ILimiterHandler>();
        public RateLimiterProxy(IResource resource) : base(resource)
        {
        }

        public override string GetResource(string uri, string clientId)
        {
            string response = String.Empty;

            bool queryHandledSuccessfully = true;
            try
            {
                queryHandledSuccessfully = handlers.Select( h => h.TryProcessRequest(clientId) ).All(result => result == true);
            }
            catch (Exception ex)
            {
                //log failure
                //TODO: think of improving exeption processing
                return Constants.HANDLER_FAILED;
            }
            if (queryHandledSuccessfully)
            {
                return base.GetResource(uri, clientId);
            }
            else
                return Constants.LIMIT_EXCEEDED;
        }

        public void SetHandlers(List<ILimiterHandler> handlers)
        {
            this.handlers = handlers;
        }
    }
}
