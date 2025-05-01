using System.Diagnostics;

namespace SWF.Core.ConsoleAccessor
{
#pragma warning disable CS0414, IDE0051
    public readonly struct TimeMeasuring
        : IDisposable
    {
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
            if (enable)
            {
                this._stopwatch = Stopwatch.StartNew();
                this._message = message;
                this._enable = enable;
            }
#endif
        }

        public void Dispose()
        {
#if DEBUG
            if (this._enable)
            {
                this._stopwatch?.Stop();
                Console.WriteLine($"[{Thread.CurrentThread.Name}] {this._message}: {this._stopwatch?.ElapsedMilliseconds} ms");
            }
#endif
            GC.SuppressFinalize(this);
        }
    }
#pragma warning restore CS0414, IDE0051
}
