using System;
using System.Threading.Tasks;
using System.Timers;

namespace RateLimiter
{
    public class EuRateLimiter : IRateLimiter
    {
        private readonly Func<Task> _doCall;
        private readonly Timer _timer;
        private bool _disposed = false;
        private object _allowedLocker = new ();
        private bool _allowed;
        public bool Allowed 
        {
            get => this._allowed;
            set 
            {
                lock (_allowedLocker) 
                {
                    this._allowed = value;
                }
            }
        }

        public EuRateLimiter(int period, Func<Task> doCall)
        {
            this._doCall = doCall;
            this.Allowed = true;
            this._timer = new();
            this._timer.AutoReset = false;
            this._timer.Elapsed += OnTimerElapsed;
            this._timer.Interval = period <= 0 ? 1000 : period;
        }

        public async Task<bool> TryCallAsync()
        {
            if (!this.Allowed) return false;

            this.Allowed = false;
            this._timer.Start();
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

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Allowed = true;
        }
    }
}
