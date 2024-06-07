using System;

namespace RateLimiter.Models;

public class RateLimitRequestModel
{
    public string AccessToken { get; set; }
    public Region? Region { get; set; }
    public DateTime DateTime { get; set; }
    public string Path { get; set; }
    public int? StatusCode { get; set; } = 200;
}