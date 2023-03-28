namespace RateLimiter.Interfaces;

public interface IMatcher
{
    public bool IsMatch(RequestInformation request);
    
    public IBucketIdentifier GetRequestId(RequestInformation request);
    
    
    public IMatcher Combine(IMatcher mather);
}