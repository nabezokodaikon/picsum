using System.Diagnostics;

namespace SWF.Core.Base
{
#pragma warning disable CS0414
    public readonly struct Measuring
        : IDisposable
    {
        private static long _threshold = long.MaxValue;

        public static void SetMeasuringThresholdMilliseconds(long threshold)
        {
            AppConstants.ThrowIfNotUIThread();

            if (threshold < 0)
            {
                return;
            }

            _threshold = threshold;
        }

        public static Measuring Time(bool enable, string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            return new Measuring(enable, message);
#else
            return default;
#endif
        }

#pragma warning disable CA1823
        private readonly Stopwatch? _stopwatch = null;
        private readonly string? _message = null;
        private readonly bool _enable = false;
#pragma warning restore CA1823

        private Measuring(bool enable, string message)
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

            if (this._enable && this._stopwatch?.ElapsedMilliseconds > -1)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | {this._stopwatch?.ElapsedMilliseconds.ToString("D4", System.Globalization.CultureInfo.InvariantCulture)} ms | {this._message} ");
            }
#endif
            GC.SuppressFinalize(this);
        }
    }
#pragma warning restore CS0414
}
