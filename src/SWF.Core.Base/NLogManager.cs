using NLog;
using NLog.Targets;

namespace SWF.Core.Base
{
    public static class NLogManager
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
                _logger = LogManager.Setup().LoadConfiguration(builder =>
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
                        Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff} | ${level:padding=-5} | ${threadid:padding=4} | ${threadname:padding=-22} | ${message:withexception=true}",
                        ArchiveFileName = Path.Combine(logDirectory, "archive", "app_{#}.log"),
                        ArchiveAboveSize = 10 * 1024 * 1024,
                        MaxArchiveFiles = 30,
                    });
                }).GetLogger("mylogger");
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
