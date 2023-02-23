using RateLimiter.Interfaces;

namespace RateLimiter.Impl
{
	public class SomeService: IService
	{
		public string GetServiceData()
		{
			return "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor...";
		}
	}
}