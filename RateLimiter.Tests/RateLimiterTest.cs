using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	private List<IRequest> _requests;

	[SetUp]
	public void Initialize()
	{
		_requests =
		[
			.. new IRequest [] {
				 new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-30) }	//  0
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-29) }	//  1 
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-28) }	//  2
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-27) }	//  3
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-26) }	//  4
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-25) }	//  5
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-24) }  //  6
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-23) }  //  7
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-22) }  //  8
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-21) }  //  9
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-20) }  // 10
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-10) }	// 11
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-9) }	// 12
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-8) }	// 13
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-7) }	// 14
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-6) }	// 15
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-5) }	// 16
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-4) }	// 17
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-3) }	// 18
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-2) }	// 19
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(-1) }	// 20
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(0) }	// 21
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(1) }	// 22
				,new Request() { IpAddress = "192.168.1.1", TimeStamp = DateTime.Now.AddSeconds(2) }	// 23
			},
		];

		Application.Initialize();

		IConfiguration configuration = new Configuration();
		Application.UnityContainer.RegisterInstance(typeof(IConfiguration), null, configuration, null);

		IStorage storage = new Storage();
		Application.UnityContainer.RegisterInstance(typeof(IStorage), null, storage, null);
	}

	[TearDown]
	public void Cleanup()
	{
		_requests.Clear();
	}

	[Test]
	public void MakeSureApplesAreAvailableAsExpected()
	{
		Apple apple1 = new Apple("apple 1");

		Assert.That(apple1.Download(_requests[0]), Is.EqualTo("apple 1"), "As expected you can have only 3 apples.");
		Assert.That(apple1.Download(_requests[2]), Is.EqualTo("apple 1"), "As expected you can have only 2 apples.");
		Assert.That(apple1.Download(_requests[4]), Is.EqualTo("apple 1"), "As expected you can have only 1 apples.");
		Assert.That(apple1.Download(_requests[5]), Is.Null, "That's it for you.");
	}

	[Test]
	public void MakeSureOrangesAreAvailableAsExpected()
	{
		Orange orange1 = new Orange("orange 1");

		Assert.That(orange1.Download(_requests[0]), Is.EqualTo("orange 1"), "As expected you can have only 2 oranges.");
		Assert.That(orange1.Download(_requests[1]), Is.EqualTo("orange 1"), "As expected you can have only 1 oranges.");
		Assert.That(orange1.Download(_requests[3]), Is.Null, "That's it for you.");
	}

	[Test]
	public void MakeSurePotatoesAreAvailableAsExpected()
	{
		Potato potato1 = new Potato("potato 1");

		Assert.That(potato1.Download(_requests[0]), Is.EqualTo("potato 1"), "As expected you can have only 1 potato.");
		Assert.That(potato1.Download(_requests[4]), Is.Null, "That's it for you.");
	}
}