using System.Diagnostics;
using System.Globalization;

namespace SWF.Core.Base
{
    public static class BootTimeMeasurement
    {
        private static readonly Process _currentProcess
            = Process.GetCurrentProcess();

        private static Stopwatch? _stopwatch = null;

        public static void Start()
        {
            AppConstants.ThrowIfNotUIThread();

            _currentProcess.PriorityClass = ProcessPriorityClass.High;

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
            NLogManager.GetLogger().Info($"Boot End: {_stopwatch.ElapsedMilliseconds} ms");

            _currentProcess.PriorityClass = ProcessPriorityClass.Normal;
        }
    }
}
