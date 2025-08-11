using NLog;
using System.Diagnostics;

namespace SWF.Core.Base
{
    public sealed class BootTimeMeasurement
    {
        private static readonly Logger LOGGER = Log.GetLogger();
        private static BootTimeMeasurement? _instance;

        public static void Start()
        {
            if (!AppConstants.IsUIThread())
            {
                throw new InvalidOperationException("UIスレッド以外から呼び出されました。");
            }

            _instance ??= new();
        }

        public static void Stop()
        {
            if (!AppConstants.IsUIThread())
            {
                throw new InvalidOperationException("UIスレッド以外から呼び出されました。");
            }

            _instance?.Stop_();
        }

        private readonly Stopwatch _sw;
        private long _isRunning = 0;

        private bool IsRunning
        {
            get
            {
                return Interlocked.Read(ref this._isRunning) == 1;
            }
            set
            {
                Interlocked.Exchange(ref this._isRunning, Convert.ToInt64(value));
            }
        }

        private BootTimeMeasurement()
        {
            this.IsRunning = true;
            this._sw = Stopwatch.StartNew();
        }

        private void Stop_()
        {
            if (this.IsRunning)
            {
                this._sw?.Stop();
                ConsoleUtil.Write(true, $"{this._sw?.ElapsedMilliseconds.ToString("D4")} ms | Boot End");
                LOGGER.Info($"Boot End: {this._sw?.ElapsedMilliseconds} ms");
                this.IsRunning = false;
            }
        }
    }
}
