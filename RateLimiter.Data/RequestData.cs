using System;
using System.Collections.Generic;

namespace RateLimiter.DataModel
{
    public class RequestData
    {
        public List<ClientResourceRuleMapping> Mappings { get; set; }
        public List<ClientRequestHistory> HistoryData { get; set; }
        public ClientRequest ClientRequest { get; set; }
        public DateTime RequestedTime { get; set; }
    }
}
