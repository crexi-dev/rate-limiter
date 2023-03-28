namespace RateLimiter;

public class ClientIdentity
{
    public ClientIdentity(string clientIp)
    {
        ClientIp = clientIp;
    }

    public string ClientIp { get; set; }
    
    public string Token { get; set; }

    public Region Region { get; set; }
}