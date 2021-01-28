using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class APIRequest
    {
        private String Token;
        private DateTime CurrentRequestTime;
        private Dictionary<String, APIEndPoint> EndPointCalls;

        private APIRequest(){}
        public APIRequest(String RequestToken, DateTime RequestTime)
        {
            this.Token = RequestToken;
            this.CurrentRequestTime = RequestTime;
            this.EndPointCalls = new Dictionary<String, APIEndPoint>();
        }

        public APIEndPoint GetEndPoint(String EndPointId, String Token, DateTime RequestTime)
        {
            APIEndPoint RequestedEndPoint;

            if (this.EndPointCalls.TryGetValue(EndPointId, out RequestedEndPoint))
            {
                RequestedEndPoint.Update(RequestTime);
            }
            else
            {
                RequestedEndPoint = new APIEndPoint(EndPointId, Token, RequestTime);
                this.EndPointCalls.Add(Token, RequestedEndPoint);
            }

            return RequestedEndPoint;
        }



        public void Update(DateTime RequestTime)
        {
            this.CurrentRequestTime = RequestTime;
        }
        
        public void AddEndPoint(String EndpointId, APIEndPoint CurrentEndPoint)
        {
            this.EndPointCalls.Add(EndpointId, CurrentEndPoint);
        }
    }
}
