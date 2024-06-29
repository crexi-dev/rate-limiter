using System;

public interface IRateLimitRule
{
    bool IsRequestAllowed(string clientId, string resourceId);
    void RecordRequest(string clientId, string resourceId);
}
