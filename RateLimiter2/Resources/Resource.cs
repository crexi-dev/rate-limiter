using RateLimiter.Rules;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Resources
{
    public class Resource
    {
        // absolute path to the resource
        public string Path { get; set; } = string.Empty;

        // if more than one verb applies to the resource, create resource(s) for other verb(s)
        public string HttpVerb { get; set; } = string.Empty;

        // rules to apply to this resource
        public IEnumerable<BaseRule> Rules = new List<BaseRule>();

        // key to the resource
        public string Key { get { return $"{Path}:{HttpVerb}"; } }

        public Resource() { }

        public Resource(string path, string httpVerb)
        {
            Path = path;
            HttpVerb = httpVerb;
        }
    }
}
