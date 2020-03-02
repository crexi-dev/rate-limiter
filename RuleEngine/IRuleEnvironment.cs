using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine
{
    public interface IRuleEnvironment
    {
        T GetFact<T>(string name);
    }
}
