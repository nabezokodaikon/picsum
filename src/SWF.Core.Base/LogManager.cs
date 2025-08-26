using Microsoft.Extensions.Logging;
using Utf8StringInterpolation;
using ZLogger;

namespace SWF.Core.Base
{
    public static class LogManager
    {
        private static ILoggerFactory? _loggerFactory = null;
        private static ILogger? _logger = null;

        public static void Initialize(string logDirectory)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(logDirectory, nameof(logDirectory));

            if (_loggerFactory != null)
            {
                throw new InvalidOperationException("ロガーは初期化されています。");
            }

            using (TimeMeasuring.Run(true, "LogManager Initialize"))
            {
                _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddZLoggerRollingFile(options =>
                    {
                        options.CaptureThreadInfo = true;
                        options.FilePathSelector = (dt, index) => Path.Combine(logDirectory, $"{dt:yyyy-MM-dd}_{index}.log");
                        options.RollingSizeKB = 10 * 1024;
                        options.UsePlainTextFormatter(formatter =>
                        {
                            formatter.SetPrefixFormatter(
                                $"{0} | {1,-11} | {2,4} | {3,-22} | ", (in MessageTemplate template, in LogInfo info) =>
                                    template.Format(info.Timestamp, info.LogLevel, info.ThreadInfo.ThreadId, info.ThreadInfo.ThreadName));
                            formatter.SetExceptionFormatter((writer, ex) => Utf8String.Format(writer, $"{ex}"));
                        });
                    });

#if DEBUG
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
#else
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
#endif
                });

                _logger = _loggerFactory.CreateLogger("global");
            }
        }

        public static void Dispose()
        {
            if (_loggerFactory == null)
            {
                throw new InvalidOperationException("ロガーファクトリーが初期化されていません。");
            }

            _loggerFactory.Dispose();
        }

        public static ILogger GetLogger()
        {
            if (_logger == null)
            {
                throw new InvalidOperationException("ロガーが初期化されていません。");
            }

            return _logger;
        }
    }
}
