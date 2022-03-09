namespace RateLimiter.Models.Contract
{
    public abstract record BaseResponse(int Count, long Size, double RequestCharge);
}
