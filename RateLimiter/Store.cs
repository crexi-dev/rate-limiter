using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
	public class Store : IStore
	{
		private readonly IDictionary<string, IList<DateTime>> _dictionary = new Dictionary<string, IList<DateTime>>();
		
		public IList<DateTime> Get(string token)
		{
			return _dictionary.ContainsKey(token) ? _dictionary[token] : new List<DateTime>();
		}

		public void Save(string token)
		{
			if(!_dictionary.ContainsKey(token))
				_dictionary.Add(token, new List<DateTime>());

			_dictionary[token].Add(DateTime.UtcNow);
		}
	}
}
