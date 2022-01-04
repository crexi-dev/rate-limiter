using Moq;
using NUnit.Framework;
using RateLimiter.TimeStamp;
using System;
using Xunit;

namespace RateLimiter.Tests
{
    //[TestFixture]
    //public class RateLimiterTest
    //{
    //    [Test]
    //    public void Example()
    //    {
    //        Assert.IsTrue(true);
    //    }
    //}

    public class RateLimiterTests
    {
        private readonly Mock<ITimestamp> _mockTimestamp = new Mock<ITimestamp>();

        private long GetElapsedTimeInTicks(int elapsedTimeMs)
        {
            return elapsedTimeMs * TimeSpan.TicksPerMillisecond;
        }

        /*
         * Calculates the minimum elapsed time required for a request to conform, assuming the previous window met the request limit
         * This equation is based on the one used by SlidingWindow
         */
        private long GetMinimumElapsedTimeInTicks(int requestLimit, int requestIntervalMs, int currentRequestCount)
        {
            return GetElapsedTimeInTicks(-1 * (requestIntervalMs * (requestLimit - currentRequestCount - 1) / requestLimit - requestIntervalMs));
        }

        private long SaturateWindow(RateLimiter slidingWindow, int requestLimit, long incrementTicks, long timeElapsedTicks)
        {
            for (int i = 0; i < requestLimit; i++)
            {
                _mockTimestamp
                    .Setup(x => x.GetTimestamp())
                    .Returns(timeElapsedTicks);

                Assert.True(slidingWindow.RequestConforms());

                timeElapsedTicks += incrementTicks;
            }

            Assert.False(slidingWindow.RequestConforms());

            return timeElapsedTicks;
        }

        [Xunit.Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void SlidingWindow_InvalidConstructorArguments_ThrowsArgumentException(int requestLimit, int requestIntervalMs)
        {
            Assert.Throws<ArgumentException>(() => new RateLimiter(_mockTimestamp.Object, requestLimit, requestIntervalMs));
        }

        [Fact]
        public void RequestConforms_InitialWindow_LimitedByUnweightedRequestCount()
        {
            var requestIntervalMs = 1000;
            var requestLimit = 10;
            var slidingWindow = new RateLimiter(_mockTimestamp.Object, requestLimit, requestIntervalMs);

            SaturateWindow(slidingWindow, requestLimit, 0, 0);
        }

        [Xunit.Theory]
        [InlineData(5, 1000)]
        [InlineData(50, 1500)]
        [InlineData(100, 2000)]
        public void RequestConforms_PopulatePreviousAndCurrentWindows_PreviousWindowImpactsCurrentWindow(int requestLimit, int requestIntervalMs)
        {
            var incrementTicks = GetElapsedTimeInTicks(requestIntervalMs / requestLimit);
            var slidingWindow = new RateLimiter(_mockTimestamp.Object, requestLimit, requestIntervalMs);

            long timeElapsedTicks = SaturateWindow(slidingWindow, requestLimit, incrementTicks, 0);

            var minimumElapsedTime = GetMinimumElapsedTimeInTicks(requestLimit, requestIntervalMs, 0);

            /*
             * Increment the elapsed time by some value less than the minimum elapsed time (1/2 is used here) calculated above
             * If the rate limiter is functioning properly, the previous window's request count will prevent the next incoming request from conforming
            */
            timeElapsedTicks += minimumElapsedTime / 2;

            _mockTimestamp
                .Setup(x => x.GetTimestamp())
                .Returns(timeElapsedTicks);

            Assert.False(slidingWindow.RequestConforms());

            /*
             * Increment the elapsed time again so the next the minimum elapsed time will have passed when the call to get the time is made
             * If the rate limiter is functioning properly, the request will conform
             */
            timeElapsedTicks += minimumElapsedTime / 2;

            _mockTimestamp
                .Setup(x => x.GetTimestamp())
                .Returns(timeElapsedTicks);

            Assert.True(slidingWindow.RequestConforms());

            // Because we have not advanced the elapsed time, the next request should not conform
            Assert.False(slidingWindow.RequestConforms());
        }

        [Fact]
        public void RequestConforms_HitWindowLimitThenDelayByTwoWindowLengths_PreviousWindowDoesNotImpactCurrent()
        {
            var requestIntervalMs = 1000;
            var requestLimit = 50;
            var incrementTicks = GetElapsedTimeInTicks(requestIntervalMs / requestLimit);
            var slidingWindow = new RateLimiter(_mockTimestamp.Object, requestLimit, requestIntervalMs);

            // Saturate initial window and hit request limit
            long timeElapsedTicks = SaturateWindow(slidingWindow, requestLimit, incrementTicks, 0);

            // Delay by twice the window length to ensure the previous window has no impact
            timeElapsedTicks += GetElapsedTimeInTicks(requestIntervalMs * 2);

            // Verify that the current window is unimpacted by the previously tracked window
            SaturateWindow(slidingWindow, requestLimit, incrementTicks, timeElapsedTicks);
        }
    }
}
