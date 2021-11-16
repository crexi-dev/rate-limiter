using System;
using System.Threading.Tasks;
using System.Timers;

namespace RateLimiter
{
    public abstract class RateLimiter : IRateLimiter
    {
        private readonly Func<Task> _doCall;
        private readonly Timer _timer;
        private bool _disposed = false;

        protected int period;
        public bool Allowed { get; private set; }

        public RateLimiter(Func<Task> doCall)
        {
            this._doCall = doCall;
            this.Allowed = true;
            this._timer = new();
            this._timer.AutoReset = false;
            this._timer.Elapsed += OnTimerElapsed;
        }

        public async Task<bool> TryCallAsync()
        {
            if (!this.Allowed) return false;

            this.Allowed = false;
            this._timer.Interval = this.period;
            this._timer.Start();
            await this._doCall();
            return true;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Allowed = true;
        }

        public void Dispose()
        {
            if (!this._disposed) {
                this._timer.Dispose();
            }
        }
    }
}
