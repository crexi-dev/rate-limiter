namespace RateLimiter.FixedCapacityByCountryPolicy
{
    public record class FixedCapacityByCountryItemMiddleWareOptions
    {
        public string Name { get; set; }
        public uint Amount { get; set; }
        public uint TimeSpan { get; set; }
    }
}
