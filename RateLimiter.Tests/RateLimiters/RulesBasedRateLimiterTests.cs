using NSubstitute;
using NUnit.Framework;
using RateLimiter.RateLimiters;
using System.Threading.Tasks;

namespace RateLimiter.Tests.RateLimiters;

[TestFixture]
public sealed class RulesBasedRateLimiterTests
{
	private RulesBasedRateLimiter? Target { get; set; }
	private IRulesStorage? RulesStorage { get; set; }
	private IClientsStorage? ClientsStorage { get; set; }

	[SetUp]
	public void SetUp()
	{
		RulesStorage = Substitute.For<IRulesStorage>();
		ClientsStorage = Substitute.For<IClientsStorage>();

		Target = new RulesBasedRateLimiter(RulesStorage, ClientsStorage);
	}

	[Test]
	public async Task should_return_false_for_unknown_client()
	{
		//arrange
		//act
		var actual = await Target!.AllowRequest("resource", "client");
		//assert
		Assert.That(actual, Is.False);
		await RulesStorage!.DidNotReceive().GetRule(Arg.Any<string>());
	}

	[Test]
	[TestCase(true)]
	[TestCase(false)]
	public async Task should_return_value_from_rule(bool allows)
	{
		//arrange
		const string resource = "resource", clientToken = "client";

		var client = new Client("any region");
		ClientsStorage!.GetClient(clientToken).Returns((true, client));

		var rule = Substitute.For<IRule>();
		rule.Allows(client).Returns(allows);
		RulesStorage!.GetRule(resource).Returns(rule);
		//act
		var actual = await Target!.AllowRequest(resource, clientToken);
		//assert
		Assert.That(actual, Is.EqualTo(allows));
	}
}
