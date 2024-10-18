using System.Diagnostics;

namespace SWF.Core.Base
{
    public readonly struct TimeMeasuring
        : IDisposable
    {
        public static TimeMeasuring Run(bool enable, string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            return new TimeMeasuring(enable, message);
#elif DEVELOP
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            return new TimeMeasuring(enable, message);
#elif RELEASE
            return default;
#endif
        }

        private readonly Stopwatch? stopwatch = null;
        private readonly string? message = null;
        private readonly bool enable = false;

        private TimeMeasuring(bool enable, string message)
        {
#if DEBUG
            this.stopwatch = Stopwatch.StartNew();
            this.message = message;
            this.enable = enable;
#elif DEVELOP
            this.stopwatch = Stopwatch.StartNew();
            this.message = message;
            this.enable = enable;
#endif
        }

        public void Dispose()
        {
#if DEBUG
            this.stopwatch?.Stop();
            ConsoleUtil.Write(this.enable, $"{this.message}: {this.stopwatch?.ElapsedMilliseconds} ms");
#elif DEVELOP
            this.stopwatch?.Stop();
            ConsoleUtil.Write(this.enable, $"{this.message}: {this.stopwatch?.ElapsedMilliseconds} ms");
#endif
        }
    }
}
