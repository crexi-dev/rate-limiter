using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        RateLimiterManager manager = new RateLimiterManager();

        [Test]
        //A function is called more than 100 times over 60 seconds
        public void FunctionLimitedNumOfCalls()
        {
            var counter = 0;
            var executed = false;
            var watch = new System.Diagnostics.Stopwatch();
            var maxActionsAllowed = 100;

            do
            {
                executed = manager.Check("function1", //function name
                        "User1",  // Identifier
                        new System.TimeSpan(0, 0, 60),  // period of 60 seconds
                        maxActionsAllowed); //max actions in period
                if (executed)
                    counter++;
            } while (executed);

            Assert.AreEqual(maxActionsAllowed, counter); // 100 actions are allowed            
        }


        [Test]
        //A function allows a call every 2 seconds
        public void FunctionLimitedTimeBetweenCalls()
        {
            var counter = 0;
            var executed = false;
            var maxActionsAllowed = 10;

            do
            {
                executed = manager.Check("function2",  //function name
                    "User2",  // Identifier
                    new System.TimeSpan(1, 0, 0), //period 1 hour
                    maxActionsAllowed,
                    new System.TimeSpan(0, 0, 2)); // 2 seconds "time between each action"
                if (executed)
                {
                    counter++;
                    System.Threading.Thread.Sleep(1000); //put to sleep the thread for 1 second
                }
            } while (executed);

            Assert.AreEqual(1, counter); //only 1 action is allowed every 2 seconds.
        }

        [Test]
        //A function allows a call every 2 seconds with a max of 10 calls in 30 minutes
        // in this case, add a 3 seconds delay after every call.
        public void FunctionLimitedTimeBetweenCallsWithDelay()
        {
            var counter = 0;
            var executed = false;
            var maxActionsAllowed = 10;

            do
            {
                executed = manager.Check("function3",  //function name
                    "User3",  // Identifier
                    new System.TimeSpan(0, 30, 0), //period 30 minutes
                    maxActionsAllowed,
                    new System.TimeSpan(0, 0, 2)); // time between each action
                if (executed)
                {
                    counter++;
                    System.Threading.Thread.Sleep(3000); //delay for 3 seconds
                }
            } while (executed);

            Assert.AreEqual(10, counter); //we have 10 actions allowed in 30 minutes because each action is called after 3 seconds
        }


        [Test]
        // A function allows a max of 10 calls in 10 seconds
        // if a call is received every 2 seconds, we have 5 calls in 10 seconds, this is why we can continue doing more calls
        public void FunctionLimitedNumOfCallsWithDelay()
        {
            var counter = 0;
            var executed = false;
            var maxActionsAllowed = 10;

            do
            {
                executed = manager.Check("function4",  //function name
                    "User4",  // Identifier
                    new System.TimeSpan(0, 0, 10), //period 20 seconds
                    maxActionsAllowed);
                if (executed)
                {
                    counter++;
                    System.Threading.Thread.Sleep(2000); //delay for 2 second
                }
            } while (executed && counter < 11);

            Assert.Greater(counter, maxActionsAllowed);
        }
    }
}
