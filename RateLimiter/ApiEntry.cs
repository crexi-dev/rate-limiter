using RateLimiter.Api.Queries;
using RateLimiter.Domain.ApiLimiter;
using RateLimiter.Domain.Resource;
using RateLimiter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class ApiEntry
    {
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
            // Normally would use something like MediatR here, but want to keep this project simple
            return _searchQuery.Execute(token);
        }

        public string Update(string token)
        {
            // Normally would use something like MediatR here, but want to keep this project simple
            return _updateCommand.Execute(token);
        }
    }
}
