using System.Diagnostics;

namespace SWF.Core.Base
{
    public sealed partial class TimeMeasuring
        : IDisposable
    {
        public static TimeMeasuring Run(string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            return new TimeMeasuring(message);
#elif DEVELOP
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            return new TimeMeasuring(message);
#elif RELEASE
            return new TimeMeasuring();
#endif
        }

        private readonly Stopwatch? stopwatch = null;
        private readonly string? message = null;

        private TimeMeasuring()
        {
            this.stopwatch = null;
            this.message = null;
        }

        private TimeMeasuring(string message)
        {
#if DEBUG
            this.stopwatch = Stopwatch.StartNew();
            this.message = message;
#elif DEVELOP
            this.stopwatch = Stopwatch.StartNew();
            this.message = message;
#endif
        }

        public void Dispose()
        {
#if DEBUG
            this.stopwatch?.Stop();
            ConsoleUtil.Write($"{this.message}: {this.stopwatch?.ElapsedMilliseconds} ms");
#elif DEVELOP
            this.stopwatch?.Stop();
            ConsoleUtil.Write($"{this.message}: {this.stopwatch?.ElapsedMilliseconds} ms");
#endif
        }
    }
}
