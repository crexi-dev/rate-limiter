using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RequestManager
    {
        private Dictionary<String, APIRequest> RequestDirctionary = new Dictionary<string, APIRequest>();
        public APIRequest CreateRequest(String Token)
        {
            APIRequest CurrentRequest;

            if (this.RequestDirctionary.TryGetValue(Token, out CurrentRequest))
            {
                CurrentRequest.UpdateRequest(DateTime.Now);
            }
            else
            {
                CurrentRequest = new APIRequest(Token, DateTime.Now);
                this.RequestDirctionary.Add(Token, CurrentRequest);
            }

            return CurrentRequest;
        }
    }
}
