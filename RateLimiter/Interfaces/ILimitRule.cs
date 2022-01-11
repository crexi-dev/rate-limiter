using Microsoft.AspNetCore.Http;

public interface ILimitRule
{
    bool ExecuteRule(HttpContext context);
    void CollectRequests(HttpContext context);
}
