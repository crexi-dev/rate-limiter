using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Models;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;

namespace RateLimiter.Executors
{
    public class RuleExecutor : IRuleExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public RuleExecutor(IServiceProvider serviceProvider, IMapper mapper)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public bool ExecuteRules(IEnumerable<RuleExecuteRequestModel> ruleExecuteRequestModels, string token)
        {
            foreach (var ruleExecuteModel in ruleExecuteRequestModels)
            {
                var result = ExecuteRule(ruleExecuteModel, token);

                if (!result)
                    return false;
            }

            return true;
        }

        public bool ExecuteRule(RuleExecuteRequestModel ruleExecuteRequestModel, string token)
        {
            var type = Type.GetType("RateLimiter.Rules." + ruleExecuteRequestModel.Name);

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var memoryCache = _serviceProvider.GetService<IMemoryCache>();
            var instance = Activator.CreateInstance(type, memoryCache);

            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var method = instance.GetType().GetMethod(nameof(RateLimitRule.IsValid));

            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var model = _mapper.Map<RateLimitRuleModel>(ruleExecuteRequestModel);

            var paramArray = new object[] { model, token };

            var result = false;

            try
            {
                result = (bool)method.Invoke(instance, paramArray);
            }
            catch
            { }


            return result;
        }

    }
}
