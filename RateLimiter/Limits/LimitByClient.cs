using System.Collections.Generic;

namespace RateLimiter.Limits
{
    public class LimitByClient<T> : ILimitByClient
        where T : LimitBase, new()
    {
        private readonly Dictionary<string, T> _clientLimits = new Dictionary<string, T>();
        private readonly LimitBase.LimitParametersBase _parameters;

        public LimitByClient(LimitBase.LimitParametersBase parameters)
        {
            this._parameters = parameters;
        }

        public bool CanInvoke(string clientId)
        {
            if (!_clientLimits.TryGetValue(clientId, out T limit))
            {
                limit = new T {Parameters = _parameters};
                _clientLimits[clientId] = limit;
            }

            return limit.CanInvoke();
        }
    }
}
