using NLog;
using NLog.Config;
using NLog.Targets;

namespace SWF.Core.ConsoleAccessor
{
    public static class LogUtil
    {
        public const string NLOG_PROPERTY = "task";

#pragma warning disable CS8618
        public static Logger Logger { get; private set; }
#pragma warning restore CS8618

        public static void Initialize(string logDirectory)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(logDirectory, nameof(logDirectory));

            ConsoleUtil.Write(true, $"Log.Initialize Start");

            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(logDirectory, "app.log"),
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff} | ${level:padding=-5} | ${scopeproperty:item=" + NLOG_PROPERTY + "} | ${message:withexception=true}",
                ArchiveFileName = string.Format("{0}/{1}", logDirectory, "${date:format=yyyyMMdd}/{########}.log"),
                ArchiveAboveSize = 10 * 1024 * 1024,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
            };
#if DEBUG
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
#else
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
#endif
            LogManager.Configuration = config;

            Logger = LogManager.GetCurrentClassLogger();

            ConsoleUtil.Write(true, $"Log.Initialize End");
        }
    }
}
