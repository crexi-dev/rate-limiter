using System;
using System.Threading.Tasks;
using System.Timers;

namespace RateLimiter
{
    public class UsRateLimiter : IRateLimiter
    {
        private readonly Func<Task> _doCall;
        private readonly Timer _timer;
        private readonly int _requestsPerSeconds;
        private bool _disposed = false;
        private int _counter = 0;
        private object _counterLocker = new ();

        public bool Allowed => this._counter < this._requestsPerSeconds;
        public UsRateLimiter(int requestsPerSecond, Func<Task> doCall)
        {
            this._doCall = doCall;
            this._requestsPerSeconds = requestsPerSecond <= 0 ? 1 : requestsPerSecond;
            this._timer = new();
            this._timer.AutoReset = true;
            this._timer.Elapsed += OnTimerElapsed;
            this._timer.Interval = 1000;
            this._timer.Start();
        }

        public async Task<bool> TryCallAsync()
        {
            if (!this.Allowed) return false;
            this.IncrementCounter();
            await this._doCall();
            return true;
        }

        public void Dispose()
        {
            if (!this._disposed)
            {
                this._timer.Dispose();
            }
        }

        private void IncrementCounter() 
        {
            lock (this._counterLocker) 
            {
                this._counter++;
            }
        }

        private void ResetCounter() 
        {
            lock (this._counterLocker) 
            {
                this._counter = 0;
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.ResetCounter();
        }
    }
}
