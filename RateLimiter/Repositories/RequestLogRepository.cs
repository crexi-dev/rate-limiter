using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Repositories;

public class RequestLogRepository : IRequestLogRepository
{
    private Dictionary<RequestKey, RequestValue> _requestLog = new Dictionary<RequestKey, RequestValue>();
    public int GetRequestNumber(string resource, DateTime timestamp, Guid clientId, int timeSpanInSeconds)
    {
        if(_requestLog.TryGetValue(new RequestKey(resource, clientId), out RequestValue? requestValue))
        {
            return requestValue.Where(x => x.Timestamp > timestamp.AddSeconds(-timeSpanInSeconds)).Count();
        }
        return 0;
    }

    public void Log(Request token, bool result)
    {
        lock (_requestLog)
        {
            RequestKey key = new RequestKey(token.Resource, token.Token.ClientId);
            if (_requestLog.TryGetValue(key, out var value)){
                value.Add(new RequestRecord { Timestamp = token.Timestamp, Result = result });
            }
            else
            {
                _requestLog[key] = new RequestValue { new RequestRecord { Timestamp = token.Timestamp, Result = result } };
            }
        }
        
    }

}
