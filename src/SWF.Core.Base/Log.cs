using NLog;
using NLog.Targets;

namespace SWF.Core.Base
{
    public static class Log
    {
        private static Logger? _logger = null;

        public static void Initialize(string logDirectory)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(logDirectory, nameof(logDirectory));

            if (_logger != null)
            {
                throw new InvalidOperationException("ロガーは初期化されています。");
            }

            using (TimeMeasuring.Run(true, "Log.Initialize"))
            {
#pragma warning disable CA2000 // スコープを失う前にオブジェクトを破棄
                _logger = new LogFactory().Setup().LoadConfiguration(builder =>
                {
                    builder.ForLogger().FilterMinLevel(
#if DEBUG
                        LogLevel.Trace
#else
                        LogLevel.Info
#endif
                    ).WriteTo(new FileTarget("logfile")
                    {
                        FileName = Path.Combine(logDirectory, "app.log"),
                        Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff} | ${level:padding=-5} | ${threadid:padding=4} | ${threadname:padding=-14} | ${message:withexception=true}",
                        ArchiveFileName = Path.Combine(logDirectory, "archive", "app_{#}.log"),
                        ArchiveAboveSize = 10 * 1024 * 1024,
                        MaxArchiveFiles = 30,
                    });
                }).GetLogger("mylogger");
#pragma warning restore CA2000
            }
        }

        public static Logger GetLogger()
        {
            if (_logger == null)
            {
                throw new InvalidOperationException("ロガーが初期化されていません。");
            }

            return _logger;
        }
    }
}
