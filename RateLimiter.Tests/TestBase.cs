using System.Threading.Tasks;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TestBase
    {
        [SetUp]
        public void Init()
        {
            DataStore.DataStore.ClearDataStore();
        }
    }
}