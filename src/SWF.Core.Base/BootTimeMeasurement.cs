using System.Diagnostics;
using System.Globalization;
using ZLogger;

namespace SWF.Core.Base
{
    public static class BootTimeMeasurement
    {
        private static Stopwatch? _stopwatch = null;

        public static void Start()
        {
            AppConstants.ThrowIfNotUIThread();

            if (_stopwatch != null)
            {
                throw new InvalidOperationException("起動時間の計測は開始されています。");
            }

            _stopwatch = Stopwatch.StartNew();
        }

        public static void Stop()
        {
            AppConstants.ThrowIfNotUIThread();

            if (_stopwatch == null)
            {
                throw new InvalidOperationException("起動時間の計測は開始されていません。");
            }

            _stopwatch.Stop();
            ConsoleUtil.Write(true, $"{_stopwatch.ElapsedMilliseconds.ToString("D4", CultureInfo.InvariantCulture)} ms | Boot End");
            LogManager.GetLogger().ZLogInformation($"Boot End: {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
