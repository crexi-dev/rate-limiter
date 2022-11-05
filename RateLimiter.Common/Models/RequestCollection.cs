using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RequestCollection
    {

        public string Key { get; private set; }
        public List<RequestEvent> Events { get; set; }


        public RequestCollection(string requestKey)
        {
            Key = requestKey;
            Events = new List<RequestEvent>();
        }
    }




}
