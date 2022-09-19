namespace RateLimiter.Rules
{
    /// <summary>
    /// The class is made for demo of using rules combination
    /// </summary>
    public class OddOrEvenSizeByPeriodLimiteRule : ILimiteRule
    {
        private RequestTotalSizePeriodLimiteRule oddNumbersRule;
        private RequestTotalSizePeriodLimiteRule evenNumbersRule;

        public OddOrEvenSizeByPeriodLimiteRule(long _periodMs, long _sizeLimit)
        {
            oddNumbersRule = new RequestTotalSizePeriodLimiteRule(_periodMs, _sizeLimit);
            evenNumbersRule = new RequestTotalSizePeriodLimiteRule(_periodMs, _sizeLimit);
        }

        public bool CanPassNow(Request request)
        {
            return request.Size % 2 == 0 
                ? oddNumbersRule.CanPassNow(request)
                : evenNumbersRule.CanPassNow(request);
        }
    }
}
