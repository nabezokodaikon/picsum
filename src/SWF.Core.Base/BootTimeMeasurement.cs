using System.Diagnostics;

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
                throw new InvalidOperationException("�N�����Ԃ̌v���͊J�n����Ă��܂��B");
            }

            _stopwatch = Stopwatch.StartNew();
        }

        public static void Stop()
        {
            AppConstants.ThrowIfNotUIThread();

            if (_stopwatch == null)
            {
                throw new InvalidOperationException("�N�����Ԃ̌v���͊J�n����Ă��܂���B");
            }

            _stopwatch.Stop();
            ConsoleUtil.Write(true, $"{_stopwatch.ElapsedMilliseconds.ToString("D4")} ms | Boot End");
            Log.GetLogger().Info($"Boot End: {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
