using RateLimiter.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class ApiEntry
    {
        private SearchQuery _searchQuery = new SearchQuery();

        public string Search(string token)
        {
            // Normally would use something like MediatR here, but want to keep this project simple
            return _searchQuery.Execute(token);
        }
    }
}
