namespace RateLimiterApi
{
	public class Auth
	{
		private static ClientCridentials clientCridentials;

		public Auth()
		{
			if (clientCridentials == null)
			{
				clientCridentials = new ClientCridentials();
			}
		}
		public static Client? Login(string password)
		{
			if (clientCridentials.Cridentials.ContainsKey(password))
			{
				return clientCridentials.Cridentials[password];
			}

			return null;
		}
	}
}
