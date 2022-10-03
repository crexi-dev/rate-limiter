using RateLimiter.Resources;
using RateLimiter.Rules.Interfaces;
using System.Net;

namespace RateLimiter.Rules
{
    public abstract class BaseRule : IComparable<BaseRule>, IRateRule
    {
        public string Name { get; set; } = String.Empty;
        public int SortOrder { get; set; }  

        public virtual bool Evaluate(Resource resource, string clientToken, List<DateTime> requests, ref bool terminateRuleProcessing)
        {
            throw new NotImplementedException();
        }

        int IComparable<BaseRule>.CompareTo(BaseRule? other)
        {
            if (other == null) return 1;
            return SortOrder.CompareTo(other.SortOrder);
        }
    }
}
