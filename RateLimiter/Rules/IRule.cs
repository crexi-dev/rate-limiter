namespace RateLimiter.Rules;

public interface IRule
{
    /// <summary>
    /// Method that checks that request could be accepted or not and modifying registration info in storage 
    /// </summary>
    /// <param name="userRegInfo">a registered info about token from storage</param>
    /// <returns></returns>
    bool CheckAndUpdate(UserRegInfo userRegInfo);
}