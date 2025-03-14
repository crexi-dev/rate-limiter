using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public interface IConfigLoader
    {
        IEnumerable<RuleDefinition> LoadRules(ContextDto contextDto);
    }

    internal class ConfigLoader : IConfigLoader
    {
        public IEnumerable<RuleDefinition> LoadRules(ContextDto contextDto)
        {
            var json = File.ReadAllText("config.json");//hardcoded path and another types of hardcode are bad in real project, but it can be another way to configure rules
            var section = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<RuleDefinition>>>>(json) ?? new Dictionary<string, Dictionary<string, List<RuleDefinition>>>();
            var result = section.Where(x => x.Key == contextDto.Path)
                .SelectMany(x => x.Value)
                .Where(x => x.Key == contextDto.Region)
                .SelectMany(x => x.Value)
                .Where(x => x.Variables.All(v => contextDto.Id.Contains(v.Value)))
                .ToList();

            return result;

        }
    }


}
