using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterApi
{
	public class ResourceApi : IFilterRequest
	{
		readonly string limitResult = "The maximum limit of request exceeded for client Id-";
		readonly string successResult = "Success request for client Id-";

		public string ResourceOne(Client client)
		{
			return ProcessRequest(client);
		}

		public string ResourceTwo(Client client)
		{
			return ProcessRequest(client);
		}

		public string ResourceThree(Client client)
		{
			return ProcessRequest(client);
		}

		public string ProcessRequest(Client client)
		{
			if (!IsValidRequest(client))
			{
				return limitResult + client.Id;
			}
			return successResult + client.Id;
		}


		public bool IsValidRequest(IUser user)
		{
			Client? client = user as Client;
			if (!AttemptsCount.Attempts.ContainsKey(client.Rule))
			{
				AttemptsCount.Attempts.Add(client.Rule, 1);
			}
			else
			{
				AttemptsCount.Attempts[client.Rule]++;
			}
			if (RuleConfig.AttemptsByRules[client.Rule] < AttemptsCount.Attempts[client.Rule])
			{
				return false;
			}
			return true;
		}
	}
}
