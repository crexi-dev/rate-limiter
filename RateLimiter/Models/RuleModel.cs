using RateLimiter.Entities;

namespace RateLimiter.Models
{
    public abstract class RuleModel
    {
        public int RuleId { get; set; }
        public RuleTypesEnum RuleType { get; set; }

        public abstract bool CheckRule(ClientModel client);
    }
}
