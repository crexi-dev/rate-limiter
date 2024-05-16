namespace RateLimiter;
public class RequestValidator
{
    private readonly IRuleRepository _ruleRepository;
    private readonly IRequestLogRepository _requestLogRepository;

    public RequestValidator(IRuleRepository ruleRepository, IRequestLogRepository requestLogRepository)
    {
        _ruleRepository = ruleRepository;
        _requestLogRepository = requestLogRepository;
    }

    public bool Validate(Request request)
    {
        var ruleCollection = _ruleRepository.GetRules(request.Token);
        var validationResult = ruleCollection.Validate(request.Token);
        _requestLogRepository.Log(request, validationResult);
        return validationResult;
    }
}
