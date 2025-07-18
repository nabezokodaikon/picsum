using NLog;
using NLog.Config;
using NLog.Targets;

namespace SWF.Core.Base
{
    public static class Log
    {
        public const string NLOG_PROPERTY = "task";

        public static void Initialize(string logDirectory)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(logDirectory, nameof(logDirectory));

            using (TimeMeasuring.Run(true, "Log.Initialize"))
            {
                var config = new LoggingConfiguration();

                var logfile = new FileTarget("logfile")
                {
                    FileName = Path.Combine(logDirectory, "app.log"),
                    Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff} | ${level:padding=-5} | ${threadid:padding=4} | ${threadname:padding=-14} | ${message:withexception=true}",
                    ArchiveFileName = Path.Combine(logDirectory, "archive", "app_{#}.log"),
                    ArchiveAboveSize = 10 * 1024 * 1024,
                    MaxArchiveFiles = 30,
                };
#if DEBUG
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
#else
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
#endif
                LogManager.Configuration = config;
            }
        }

        public static Logger GetLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
    }
}
