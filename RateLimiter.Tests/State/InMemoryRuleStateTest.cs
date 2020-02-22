using NUnit.Framework;
using RateLimiter.State;

namespace RateLimiter.Tests.State
{
    [TestFixture]
    internal class InMemoryRuleStateTest
    {
        [OneTimeSetUp]
        public void SingletonSetUp()
        {
            var mem = InMemoryRuleState.GetInstance;
            mem.Put("1", "3");
        }

        [Test]
        public void Put_NewItem()
        {
            var mem = InMemoryRuleState.GetInstance;
            mem.Put("5", "4");

            Assert.AreEqual("4", mem.Retrieve<string>("5").Item1);
        }

        [Test]
        public void Retrieve_Item()
        {
            var mem = InMemoryRuleState.GetInstance;
            Assert.AreEqual("3", mem.Retrieve<string>("1").Item1);
        }

        [Test]
        public void Retrieve_Item_NotFound()
        {
            var mem = InMemoryRuleState.GetInstance;
            Assert.IsFalse(mem.Retrieve<string>("x").Item2);
        }
    }
}