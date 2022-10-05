namespace RateLimiter.API.Middlewares;

public class CheckTokenMiddleware : IMiddleware
{
    private readonly ICheckConstraints _checkConstraints;

    public CheckTokenMiddleware(ICheckConstraints checkConstraints)
    {
        _checkConstraints = checkConstraints;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var tokenId = Guid.Parse(context.Request.Headers["TokenId"].ToString());
        if (await _checkConstraints.AccessGranted(tokenId, context.RequestAborted))
            await next.Invoke(context);
        else
            throw new Exception("Bad token");
    }
}