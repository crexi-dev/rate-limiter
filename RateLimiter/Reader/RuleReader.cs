using AutoMapper;
using Microsoft.Extensions.Options;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Reader
{
    public class RuleReader : IRuleReader
    {
        private readonly IEnumerable<RuleConfigurationModel> _ruleConfigModel;
        private readonly IMapper _mapper;

        public RuleReader(IOptions<List<RuleConfigurationModel>> ruleConfigModelOptions, IMapper mapper)
        {
            _ruleConfigModel = ruleConfigModelOptions.Value ?? throw new ArgumentNullException(nameof(ruleConfigModelOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<ReadRuleResponseModel> ReadRules(ReadRulesRequestModel readRulesRequestModel)
        {
            var ruleConfigModels = _ruleConfigModel.Where(x => x.Endpoint == readRulesRequestModel.RequestPath && x.Action == readRulesRequestModel.RequestAction);
            return _mapper.Map<IEnumerable<ReadRuleResponseModel>>(ruleConfigModels);
        }
    }
}
