using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestManager
    {
        private Dictionary<String, APIRequest> RequestDirctionary;
        
        public RequestManager()
        {
            this.RequestDirctionary = new Dictionary<string, APIRequest>();
        }

        public APIEndPoint CreateRequest(String Token, String EndPointId, DateTime RequestTime)
        {
            APIRequest CurrentRequest;
            APIEndPoint CurrentEndPoint;

            if (this.RequestDirctionary.TryGetValue(Token, out CurrentRequest))
            {
                CurrentRequest.Update(RequestTime);
                CurrentEndPoint = CurrentRequest.GetEndPoint(EndPointId, Token, RequestTime);
            }
            else
            {
                CurrentRequest = new APIRequest(Token, RequestTime);
                CurrentEndPoint = new APIEndPoint(EndPointId, Token, RequestTime);

                CurrentRequest.AddEndPoint(EndPointId, CurrentEndPoint);
                this.RequestDirctionary.Add(Token, CurrentRequest);
            }

            return CurrentEndPoint;
        }
    }
}
