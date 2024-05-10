namespace RateLimiter.Models
{
    public class ReadRulesRequestModel
    {
        public string RequestPath { get; set; }

        public string RequestAction { get; set; }
    }
}
