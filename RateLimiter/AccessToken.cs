namespace RateLimiter;

public enum Region
{
    us = 1,
    eu = 2,
}

public record AccessToken(string UserId, Region Region = Region.us);
