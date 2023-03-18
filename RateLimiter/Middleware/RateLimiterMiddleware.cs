using Microsoft.AspNetCore.Http;
using RateLimiter.Services;
using RateLimiter.Services.Rules;
using RateLimiter.Services.Rules.Models;

namespace RateLimiter.Middleware;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRulesService _rulesManager;
    public RateLimiterMiddleware(RequestDelegate next, IRulesService rulesManager)
    {
        _next = next;
        _rulesManager = rulesManager;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/swagger/")) //Swagger is only used in debug configuration, so we can pass the request further
        {
            await _next.Invoke(context);
            return;
        }

        var accessToken = context.Request.Headers["accessToken"];
        ClientInfo clientInfo;
        IRule rule;

        if (accessToken.Count == 0)
        {
            var ip = GetIP(context);
            clientInfo = new ClientInfo { Ip = ip };
            rule = _rulesManager.GetRule(clientInfo);
        }
        else
        {
            clientInfo = new ClientInfo { AccessToken = accessToken };
            rule = _rulesManager.GetRule(clientInfo);
        }

        if(rule.IsAllowed(clientInfo))
            await _next.Invoke(context);
        else
            context.Response.StatusCode = 429;
    }

    private string GetIP(HttpContext context)
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress;
        
        if (remoteIpAddress == null)
            return "";
        
        if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        return remoteIpAddress.ToString();
    }
}