using System;
using System.Collections.Generic;

public interface IRateLimitRule
{
    bool IsRequestAllowed(
        string client, 
        string resource, 
        DateTime currentTime, 
        Dictionary<(string client, string resource), List<DateTime>> clientRequests);
}
