namespace RateLimiter;

public class RequestInformation
{
    public string Url { get; }
    
    public string HttpMethod { get; }
    
    public ClientIdentity ClientId { get; }

    public RequestInformation(string url, ClientIdentity clientId, string httpMethod)
    {
        Url = url;
        ClientId = clientId;
        HttpMethod = httpMethod;
    }
}