namespace RateLimiter.Models
{
    public abstract class BaseResult
    {
        private bool isFailed = false;
        private string message;

        public bool IsFailed => isFailed;

        public string Message => message;

        public void Fail(string message)
        {
            isFailed = true;
            this.message = message;
        }
    }
}
