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
using System.Threading;
using System.Windows.Forms;

namespace PicSum.Main
{
    internal sealed class Program
        : MarshalByRefObject
    {
        private static readonly Mutex mutex = new(true, ApplicationConstants.MUTEX_NAME);

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    ResourceUtil.CreateApplicationDirectories();
                    ConfigureLog();

                    var logger = LogManager.GetCurrentClassLogger();

                    Thread.CurrentThread.Name = ApplicationConstants.UI_THREAD_NAME;

                    logger.Debug("アプリケーションを開始します。");

                    AppDomain.CurrentDomain.UnhandledException += new(CurrentDomain_UnhandledException);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var resource = new ResourceManager())
                    using (var initialForm = new InitialForm())
                    {
                        Application.Run(initialForm);
                    }

                    logger.Debug("アプリケーションを終了します。");
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

                using (var pipeClient = new NamedPipeClientStream(".", ApplicationConstants.PIPE_NAME, PipeDirection.Out))
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
            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(ResourceUtil.LOG_DIRECTORY, "app.log"),
                Layout = "${longdate} | ${level:padding=-5} | ${threadname} | ${message:withexception=true}",
                ArchiveFileName = string.Format("{0}/{1}", ResourceUtil.LOG_DIRECTORY, "${date:format=yyyyMMdd}/{########}.log"),
                ArchiveAboveSize = 10 * 1024 * 1024,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
            };

#if DEBUG
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
#else
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
#endif

            LogManager.Configuration = config;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error(ex);
            ExceptionUtil.ShowErrorDialog("An uncaught exception occurred.", ex);
        }
    }
}
