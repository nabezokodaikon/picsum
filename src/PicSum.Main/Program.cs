using NLog;
using NLog.Config;
using NLog.Targets;
using PicSum.Job.Logics;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
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
                    FileUtil.CreateApplicationDirectories();
                    ConfigureLog();

                    var logger = LogManager.GetCurrentClassLogger();

                    Thread.CurrentThread.Name = ApplicationConstants.UI_THREAD_NAME;

                    logger.Debug("アプリケーションを開始します。");

                    AppDomain.CurrentDomain.UnhandledException += new(CurrentDomain_UnhandledException);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var component = new ComponentManager())
                    using (var initialForm = new InitialForm())
                    {
                        Application.Run(initialForm);
                    }

                    UIThreadAccessor.Instance.Dispose();
                    FileIconCash.DisposeStaticResources();
                    ThumbnailGetLogic.DisposeStaticResouces();
                    ImageFileCacheUtil.DisposeStaticResources();
                    ImageFileSizeCacheUtil.DisposeStaticResources();
                    FileExportLogic.DisposeStaticResouces();

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
                FileName = Path.Combine(FileUtil.LOG_DIRECTORY, "app.log"),
                Layout = "${longdate} | ${level:padding=-5} | ${threadname} | ${message:withexception=true}",
                ArchiveFileName = string.Format("{0}/{1}", FileUtil.LOG_DIRECTORY, "${date:format=yyyyMMdd}/{########}.log"),
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
