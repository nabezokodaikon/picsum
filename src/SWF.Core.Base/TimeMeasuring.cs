using System.Diagnostics;

namespace SWF.Core.Base
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

        private readonly Stopwatch? stopwatch = null;
        private readonly string? message = null;
        private readonly bool enable = false;

        private TimeMeasuring(bool enable, string message)
        {
#if DEBUG
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
#endif
        }
    }
#pragma warning restore CS0414, IDE0051
}
