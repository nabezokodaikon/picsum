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
            _instance ??= new();
        }

        public static void Stop()
        {
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
                var message = $"Boot End: {this._sw?.ElapsedMilliseconds} ms";
                ConsoleUtil.Write(true, message);
                LOGGER.Info(message);
                this.IsRunning = false;
            }
        }
    }
}
