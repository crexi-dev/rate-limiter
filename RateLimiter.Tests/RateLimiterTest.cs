using NUnit.Framework;
using FakeItEasy;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture("Unit")]
    public class RateLimiterTest
    {
        [Test]
        public async Task When_RuleEvalTrue_ShouldCallSerice_And_AddToJournal()
        {
            //arrange
            var rule = A.Fake<IRule>();
            A.CallTo(() => rule.Eval(A<IJournal>._)).Returns(true);

            var service = A.Fake<IService>();
            A.CallTo(() => service.DoStuff()).DoesNothing();

            var journal = A.Fake<IJournal>();
            var resource = A.Fake<Resource>(x => x.WithArgumentsForConstructor(() => new Resource(new List<IRule> { rule }, journal, service)));

            //act
            await resource.Execute("US");

            //assert
            A.CallTo(() => rule.Eval(A<IJournal>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => service.DoStuff()).MustHaveHappenedOnceExactly();
            A.CallTo(() => journal.Add(A<string>._, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task When_RuleEvalFalse_ShouldNotCallService_NorAddToJournal()
        {
            //arrange
            var rule = A.Fake<IRule>();
            A.CallTo(() => rule.Eval(A<IJournal>._)).Returns(false);

            var service = A.Fake<IService>();
            A.CallTo(() => service.DoStuff()).DoesNothing();

            var journal = A.Fake<IJournal>();
            var resource = A.Fake<Resource>(x => x.WithArgumentsForConstructor(() => new Resource(new List<IRule> { rule }, journal, service)));

            //act
            await resource.Execute("US");

            //assert
            A.CallTo(() => rule.Eval(A<IJournal>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => service.DoStuff()).MustNotHaveHappened();
            A.CallTo(() => journal.Add(A<string>._, A<string>._)).MustNotHaveHappened();
        }
    }
}
