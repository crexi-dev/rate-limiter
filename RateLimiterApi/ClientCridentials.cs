using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterApi
{
	public class ClientCridentials
	{
		public Dictionary<string, Client> Cridentials { get; set; }

		public ClientCridentials()
		{
			Cridentials = new Dictionary<string, Client>()
			{
				{"pass1", new Client { Id = 5, Rule = Rules.RuleA } },
				{"pass2",new Client { Id = 15, Rule = Rules.RuleB } },
				{"pass3",new Client { Id = 25, Rule = Rules.RuleC } }
			};

		}
	}
}
