using System.Diagnostics;

namespace SWF.Core.ConsoleAccessor
{
    public static class BootTimeMeasurement
    {
        private static Stopwatch? bootTimeStopwatch = null;

        public static void StartBootTimeMeasurement()
        {
            bootTimeStopwatch = Stopwatch.StartNew();
        }

        public static void StopBootTimeMeasurement()
        {
            bootTimeStopwatch?.Stop();
            ConsoleUtil.Write(true, $"Boot time: {bootTimeStopwatch?.ElapsedMilliseconds} ms");
        }
    }
}
