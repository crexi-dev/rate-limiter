using System.Collections.Generic;

namespace RateLimiter
{
    public class Response
    {
        public string ResponseJson { get; set; }

        public bool IsSuccessful { get; set; }
        
        public List<string> Errors { get; set; }
    }
}
