using System.Diagnostics;

namespace SWF.Core.Base
{
#pragma warning disable CS0414
    public readonly struct TimeMeasuring
        : IDisposable
    {
        private const long DEFAULT_THRESHOLD = long.MaxValue;

        private static long _threshold = DEFAULT_THRESHOLD;

        public static long Threshold
        {
            get
            {
                return Interlocked.Read(ref _threshold);
            }
            set
            {
                if (value > -1)
                {
                    Interlocked.Exchange(ref _threshold, value);
                }
                else
                {
                    Interlocked.Exchange(ref _threshold, DEFAULT_THRESHOLD);
                }
            }
        }

        public static TimeMeasuring Run(bool enable, string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            return new TimeMeasuring(enable, message);
#else
            return default;
#endif
        }

        private readonly Stopwatch? _stopwatch = null;
        private readonly string? _message = null;
        private readonly bool _enable = false;

        private TimeMeasuring(bool enable, string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            this._stopwatch = Stopwatch.StartNew();
            this._message = message;

            if (enable)
            {
                this._enable = enable;
            }
#endif
        }

        public void Dispose()
        {
#if DEBUG
            this._stopwatch?.Stop();

            if (this._enable || this._stopwatch?.ElapsedMilliseconds > Threshold)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | {this._stopwatch?.ElapsedMilliseconds.ToString("D4")} ms | {this._message} ");
            }
#endif
            GC.SuppressFinalize(this);
        }
    }
#pragma warning restore CS0414
}
