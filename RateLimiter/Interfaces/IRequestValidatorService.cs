using RateLimiter.Enums;
using RateLimiter.Models;

namespace RateLimiter.Interfaces
{
    public interface IRequestValidatorService
    {
        public bool ValidateUserRequest(UserRequest request);
    }
}
