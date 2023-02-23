namespace RateLimiter.Models
{
	public class ValidateModel
	{
		public bool IsValid { get; set; } = true;
		public string ErrorMessage { get; set; } = string.Empty;
	}
}