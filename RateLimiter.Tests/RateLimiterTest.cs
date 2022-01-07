using Moq;
using NUnit.Framework;
using RateLimiter.SlidingWindowAlgorithm.Implementation;
using RateLimiter.SlidingWindowAlgorithm.TimeStampUtilities;
using System.Collections.Concurrent;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private readonly Mock<ITimestamp> _mockTimestamp = new Mock<ITimestamp>();
        private ConcurrentDictionary<string, SlidingWindow> _currentClientsWindows = new ConcurrentDictionary<string, SlidingWindow>();
        private long _timeElapsedTicks = 0;

        /// <summary>
        /// Tests trying to access some resource with access token of client, using sliding window to rate limit calls
        /// </summary>
        /// <param name="formalAccessToken">The token is just a string parameter as described in task, not a real-world access token</param>
        [Test]
        [TestCase("test-access-token-client-0")]
        [TestCase("test-access-token-client-2")]
        [TestCase("test-access-token-client-0")]
        [TestCase("test-access-token-client-0")]
        public void TryAccessResource(string formalCleintAccessToken)
        {
            bool accessGranted = false;
            SlidingWindow clientSlidingWindow;
            _currentClientsWindows.TryGetValue(formalCleintAccessToken, out clientSlidingWindow);
            if (clientSlidingWindow == null) // create new client window and initial slide
            {
                // Test values for intial slide
                var requestIntervalMilliseconds = 500;
                var requestLimit = 2;

                _timeElapsedTicks = 0;
                _mockTimestamp
                    .Setup(x => x.GetTimestamp())
                            .Returns(_timeElapsedTicks);
                ++_timeElapsedTicks;

                // Creating new instance of sliding window for currently specified client 
                var newClientSlidingWindow = new SlidingWindow(_mockTimestamp.Object, requestLimit, requestIntervalMilliseconds);
                if (_currentClientsWindows.TryAdd(formalCleintAccessToken, newClientSlidingWindow))
                {
                    accessGranted = newClientSlidingWindow.RequestConforms();
                    if (accessGranted)
                    {
                        // Access some resource with newly added client by specified formal client access token
                    }
                }
            }
            else // Check if request conforms by current client's sliding window state object
            {
                // For updating to a new window below inside dictionary
                var oldWindow = clientSlidingWindow.ShallowCopy();

                // Setting current timestamp for updaing object
                ++_timeElapsedTicks;
                _mockTimestamp.Setup(x => x.GetTimestamp()).Returns(_timeElapsedTicks);
                clientSlidingWindow.Timestamp = _mockTimestamp.Object;

                accessGranted = clientSlidingWindow.RequestConforms();
                
                if (accessGranted)
                {
                    // Access some resource with specified formal client access token
                }

                // Update current client object with recalculated values
                _currentClientsWindows.TryUpdate(formalCleintAccessToken, clientSlidingWindow, oldWindow);
            }

            // On the last call for client with access token "test-access-token-client-0" we expect FALSE, because request count limit is 2
            Assert.IsTrue(accessGranted);
        }
    }
}
