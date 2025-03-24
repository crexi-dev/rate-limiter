namespace RateLimiter.Model.Data
{
    public class ApiToken
    {
        public string Token { get; set; }
        public bool IsUSToken { get; set; } = true; //using bool for simplicity here but would probably be a country identifier for xx handling
        public bool Enabled { get; set; } = true;
    }
}
