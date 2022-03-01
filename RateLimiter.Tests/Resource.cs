using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    public class Resource : IResource
    {
        public IEnumerable<IRule> Rules { get; init; }
        public IJournal Journal { get; init; }
        public IService Service { get; init; }

        public Resource(IEnumerable<IRule> rules, IJournal journal, IService service)
        {
            this.Rules = rules;
            this.Journal = journal;
            this.Service = service;
        }

        public async Task Execute(string token)
        {
            if (!Rules.All(rule => rule.Eval(Journal)))
            {
                return;
            }
            
            await Service.DoStuff();
            Journal.Add(token, $"{nameof(Service.DoStuff)}");
        }
    }
}