using NLog;
using NLog.Config;
using NLog.Targets;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace PicSum.Main
{
    internal sealed class Program
        : MarshalByRefObject
    {
        private static readonly Mutex mutex = new(true, AppConstants.MUTEX_NAME);

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    Thread.CurrentThread.Name = AppConstants.UI_THREAD_NAME;

                    AppConstants.CreateApplicationDirectories();

                    ConfigureLog();
                    LogManager.GetCurrentClassLogger().Debug("アプリケーションを開始します。");

                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                    Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var resource = new ResourceManager())
                    using (var initialForm = new InitialForm())
                    {
                        Application.Run(initialForm);
                    }

                    LogManager.GetCurrentClassLogger().Debug("アプリケーションを終了します。");
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                var filePath = Environment.GetCommandLineArgs()
                    .FirstOrDefault(_ => FileUtil.CanAccess(_) && FileUtil.IsImageFile(_));
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                using (var pipeClient = new NamedPipeClientStream(".", AppConstants.PIPE_NAME, PipeDirection.Out))
                {
                    pipeClient.Connect();
                    using (var writer = new StreamWriter(pipeClient))
                    {
                        writer.WriteLine(filePath);
                    }
                }
            }
        }

        private static void ConfigureLog()
        {
            using (TimeMeasuring.Run(true, "Program.ConfigureLog"))
            {
                var config = new LoggingConfiguration();

                var logfile = new FileTarget("logfile")
                {
                    FileName = Path.Combine(AppConstants.LOG_DIRECTORY, "app.log"),
                    Layout = "${longdate} | ${level:padding=-5} | ${threadname} | ${message:withexception=true}",
                    ArchiveFileName = string.Format("{0}/{1}", AppConstants.LOG_DIRECTORY, "${date:format=yyyyMMdd}/{########}.log"),
                    ArchiveAboveSize = 10 * 1024 * 1024,
                    ArchiveNumbering = ArchiveNumberingMode.Sequence,
                };
#if DEBUG
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
#elif DEVELOP
                config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
#else
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
#endif
                LogManager.Configuration = config;
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Fatal(e.Exception);
            ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Fatal(ex);
            ExceptionUtil.ShowFatalDialog("Unhandled Non-UI Exception.", ex);
        }
    }
}
