namespace RateLimiter;

public class RequestContext
{
    public string ClientId { get; }
    
    public string Resource { get; }
    
    public RequestContext(string clientId, string resource)
    {
        ClientId = clientId;
        Resource = resource;
    }
}