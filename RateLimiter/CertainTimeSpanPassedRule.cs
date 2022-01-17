using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter
{
    public class CertainTimeSpanPassedRule : IRule
    {        
        private TimeSpan timeSpan = new TimeSpan(1, 0, 0);
        public bool CanAccess(string token, ClientLogsStorage storage)
        {
            ClientLog lastClientLog = storage.GetLast(token);
            if(lastClientLog == null)
            {
                return true;
            }
            if(lastClientLog.ClientLogTime < DateTime.Now - timeSpan)
            {
                return true;
            }
            return false;
        }
    }
}
