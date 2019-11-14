using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
	public class RateLimiter : IRateLimiter
	{
		protected readonly IStore _store;
		protected IEnumerable<IRule> _rules;

		public RateLimiter(IStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		public void SetRules(IEnumerable<IRule> rules)
		{
			_rules = rules;
		}

		public bool IsAllowed(string token)
		{
			if (_rules == null) return true;
			
			_store.Save(token);
			var result = _store.Get(token);
			
			foreach (var rule in _rules)
			{
				if (!rule.IsMatch(result))
					return false;
			}

			return true;
		}
	}
}
