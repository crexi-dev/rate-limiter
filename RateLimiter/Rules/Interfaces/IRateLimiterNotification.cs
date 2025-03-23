using System.Threading.Tasks;

namespace RateLimiter.Rules.Interfaces
{
    public interface IRateLimiterNotification
    {
        Task<bool> SendNotificationAsync(string recipient, string message);
    }
}
