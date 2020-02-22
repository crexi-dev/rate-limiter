namespace RateLimiter.Rules
{
    public class RequestFromCountry : IRule<RequestInfo>
    {
        private readonly string _countryConfig;

        public RequestFromCountry(string countryConfig)
        {
            _countryConfig = countryConfig;
        }

        public bool Execute(RequestInfo input)
        {
            if (_countryConfig == input.Country)
            {
                return true;
            }

            return false;
        }
    }
}