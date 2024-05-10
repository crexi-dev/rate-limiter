using AutoMapper;
using Microsoft.AspNetCore.Http;
using RateLimiter.Executors;
using RateLimiter.Models;
using RateLimiter.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Processor
{
    public class RateLimitProcessor : IRateLimitProcessor
    {
        private readonly IRuleReader _ruleReader;
        private readonly IRuleExecutor _ruleExecutor;
        private readonly IMapper _mapper;

        public RateLimitProcessor(IRuleReader ruleReader, IRuleExecutor ruleExecutor, IMapper mapper)
        {
            _ruleReader = ruleReader ?? throw new ArgumentNullException(nameof(ruleReader));
            _ruleExecutor = ruleExecutor ?? throw new ArgumentNullException(nameof(ruleExecutor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public bool Process(HttpContext httpContext)
        {
            var method = httpContext.Request.Method;
            var path = httpContext.Request.Path;

            var rules = _ruleReader.ReadRules(new ReadRulesRequestModel
            {
                RequestAction = method,
                RequestPath = path
            });

            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rules.Count() == 0)
                return true;

            var executorModel = _mapper.Map<IEnumerable<RuleExecuteRequestModel>>(rules);

            var token = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            var response = _ruleExecutor.ExecuteRules(executorModel, token);

            return response;
        }
    }
}
