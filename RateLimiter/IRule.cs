namespace RateLimiter
{
	public interface IRule
	{
		bool IsValid(IRequest payload);
	}
}
