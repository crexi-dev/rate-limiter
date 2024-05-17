using RateLimiter.Models;
using RateLimiter.Repositories;
using RateLimiter.Rules;

namespace RateLimiter;
public class RequestValidator
{
    private readonly IRuleRepository _ruleRepository;
    private readonly IRequestLogRepository _requestLogRepository;
    private readonly IRequestConverterFinder _requestConverterFinder;

    public RequestValidator(IRuleRepository ruleRepository, 
        IRequestLogRepository requestLogRepository, 
        IRequestConverterFinder requestConverterFinder)
    {
        _ruleRepository = ruleRepository;
        _requestLogRepository = requestLogRepository;
        _requestConverterFinder = requestConverterFinder;
    }

    public bool Validate(Request request)
    {
        IRuleCollection ruleCollection = _ruleRepository.GetRules(request.Resource, request.Token);
        
        var validationResult = ruleCollection.Validate(request);
        _requestLogRepository.Log(request, validationResult);
        return validationResult;
    }
}
public interface IRequestConverterFinder
{

} 
