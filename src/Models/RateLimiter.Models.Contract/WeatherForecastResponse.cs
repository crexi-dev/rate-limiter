using System.Collections.Generic;

namespace RateLimiter.Models.Contract
{
    public record WeatherForecastResponse
        (int Count, long Size, double RequestCharge, IList<WeatherForecast> Forecasts) : BaseResponse(Count, Size, RequestCharge);
}
