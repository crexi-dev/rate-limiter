using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class ClientLog
    {
        public string ClientToken { get; set; }

        public DateTime ClientLogTime { get; set; }
        
        public ClientLog(string token, DateTime time)
        {
            ClientToken = token;
            ClientLogTime = time;
        }
    }
}
