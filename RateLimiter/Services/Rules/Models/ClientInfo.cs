
namespace RateLimiter.Services.Rules.Models;

public record ClientInfo
{
    public string? AccessToken { get; set;}
    public string? Ip { get; set; }
    public string ClientId => (AccessToken ?? Ip) ?? "";
}