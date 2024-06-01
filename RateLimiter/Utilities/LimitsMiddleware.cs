using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using RateLimiter.DTOs;

namespace RateLimiter.Utilities;

public class LimitsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimits _options;
    private static readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

    public LimitsMiddleware(RequestDelegate next, IOptions<RateLimits> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    { 
        //Решил, что необходимые параметры будут передаваться через Query запроса
        BaseDTO baseInfo = new BaseDTO()
        {
            Token = context.Request.Query["tokenValue"].ToString(),
            Country = context.Request.Query["countryValue"].ToString()
        };
        //Проверка, что токен есть
        if (string.IsNullOrEmpty(baseInfo.Token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        //Проверка, что страна указана
        if (string.IsNullOrEmpty(baseInfo.Country))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Country code required");
            return;
        }
        //Проверка ограничений
        if (!IsRequestAllowed(baseInfo))
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too Many Requests");
            return;
        }

        await _next(context);
    }

     private bool IsRequestAllowed(BaseDTO baseInfo)
        {
            //Тут есть вопрос, может-ли быть ограничение, что вообще для определенной страны запрещены запросы (то если запросов небыло к системе, все равно нужно проверять ограничения ).
            //Если да, то проверку requestTimes != null нужно убрать. Но такого небыло в требованиях, поэтому я решил, что такого не может быть =)
            var now = DateTime.UtcNow;
            var key = $"{baseInfo.Country}:${baseInfo.Token}";
            var requestTimes = _requests.Where(pair => pair.Key == key).FirstOrDefault().Value;
            if (requestTimes != null)
            {
                lock (requestTimes)
                {
                    // Поиск ограничений по указанной в запросе стране
                    var rule = _options.Rules.Where(rule => rule.Country.Equals(baseInfo.Country, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (rule != null)
                    {
                        // Проверка, что если указана пауза между запросами, она соблюдается
                        if (rule.TimeFromLastCall != null)
                        {
                            // TimeFromLastCall = 7200 сек
                            // Если последний запрос был в 12:00, а сейчас 13:00 (12:00 + 2:00 > 13:00 = true) - запрос не пропускаем  
                            // Если последний запрос был в 12:00, а сейчас 14:00 (12:00 + 2:00 > 14:00 = true) - запрос не пропускаем  
                            // Если последний запрос был в 12:00, а сейчас 14:01 (12:00 + 2:00 > 14:01 = false) - запрос пропускаем  
                            if (requestTimes.Max().AddSeconds(rule.TimeFromLastCall.Value) > now)
                            {
                                return false;
                            }
                        }
                        // Проверка, что если не превышено количество запросов RequestCount за промежуток времени TimeSpanSeconds
                        if (rule.XRequestsPerTimespan != null)
                        {
                            if (requestTimes.Count(req =>
                                    req >= now.AddSeconds(-rule.XRequestsPerTimespan.TimeSpanSeconds)) >=
                                rule.XRequestsPerTimespan.RequestCount)
                            {
                                return false;
                            }
                        }
                    }

                    requestTimes.Add(now);
                }
            }
            else
            {
                _requests.TryAdd(key, new List<DateTime>() { now });
            }

            return true;
        }
}