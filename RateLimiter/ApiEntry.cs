using RateLimiter.Api.Queries;
using RateLimiter.Domain.ApiLimiter;
using RateLimiter.Infrastructure;

namespace RateLimiter
{
    public class ApiEntry
    {
        public const string SUCCESS = "Success";
        public const string FAIL = "Fail";

        private SearchQuery _searchQuery;
        private UpdateCommand _updateCommand;
        private IInMemoryRulesRepository _inMemoryRulesRepository;
        private IApiLimiter _apiLimiter;

        public ApiEntry(IInMemoryRulesRepository inMemoryRulesRepository, IApiLimiter apiLimiter)
        {
            _inMemoryRulesRepository = inMemoryRulesRepository;
            // For our example we will assume that the rules never change
            _searchQuery = new SearchQuery(apiLimiter, _inMemoryRulesRepository);
            _updateCommand = new UpdateCommand(apiLimiter, _inMemoryRulesRepository);
            _apiLimiter = apiLimiter;
        }

        public string Search(string token)
        {
            // Normally would use something like MediatR for CQRS here, but want to keep this project simple
            return _searchQuery.Execute(token) ? SUCCESS : FAIL;
        }

        public string Update(string token)
        {
            // Normally would use something like MediatR for CQRS here, but want to keep this project simple
            return _updateCommand.Execute(token) ? SUCCESS : FAIL;
        }
    }
}
