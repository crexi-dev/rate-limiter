using System.Collections.Generic;

namespace API.Infrastructure
{
    public class DataProvider
    {
        public List<Resources> Resources { get; set; } = new List<Resources>();
        public List<Rules> Rules { get; set; } = new List<Rules>();
    }

    public class Resources 
    {
        public string URL { get; set; }
        public string Rules { get; set; }
    }

    public class Rules
    {
        public string Rule { get; set; }
        public int MaxRequests { get; set; }
        public int Limit { get; set; }
    }
}
