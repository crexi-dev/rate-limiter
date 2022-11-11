using Microsoft.AspNetCore.Http;

namespace RateLimiter.AspNetCore;

public interface IParameterProvider
{
    string Resolve(HttpContext context);
}

public class DelegateParameterProvider : IParameterProvider
{
    private readonly Func<HttpContext, string> _resolver;

    public DelegateParameterProvider(Func<HttpContext, string> resolver)
    {
        _resolver = resolver;
    }


    public string Resolve(HttpContext context)
    {
        return _resolver(context);
    }
}