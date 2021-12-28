namespace RateLimiter.Models
{
    public enum RuleTypesEnum
    {
        Country = 0,
        RequestCount,
        Period,
        LastCall,
        RequestCountForCountry,
        RequestCountForCountryForPeriod,
        LastCallForCountry
    }
}
