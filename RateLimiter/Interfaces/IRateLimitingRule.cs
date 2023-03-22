namespace RateLimiter.Interfaces; 

public interface IRateLimitingRule {
    bool IsAllowed(string clientIdentifier, string resourceAccessed);
    void LogRequest(string clientIdentifier, IClientRequest requestToLog);
}