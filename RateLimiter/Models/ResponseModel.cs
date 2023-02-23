namespace RateLimiter.Models
{
	public class ResponseModel
	{
		public bool IsSuccess { get; set; } = true;
		public string Error { get; set; } = string.Empty;
		public string Data { get; set; } = string.Empty;
	}
}