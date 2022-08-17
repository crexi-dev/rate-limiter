using System.Linq;

namespace RateLimiter.Models.Conditions
{
    public sealed class HasRoleCondition : ICondition
    {
        private string role;

        public HasRoleCondition(string role)
        {
            this.role = role;
        }

        public bool IsMatch(IContext context)
        {
            return context.Roles != null && context.Roles.Contains(role);
        }
    }
}
