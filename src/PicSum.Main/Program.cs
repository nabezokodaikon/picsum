using NLog;
using NLog.Config;
using PicSum.Job.Logics;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;

namespace PicSum.Main
{
    [SupportedOSPlatform("windows")]
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
#if DEBUG
                    LogManager.Configuration = new XmlLoggingConfiguration(
                        Path.Combine(FileUtil.EXECUTABLE_DIRECTORY, "NLog.debug.config"));
#else
                    LogManager.Configuration = new XmlLoggingConfiguration(
                        Path.Combine(FileUtil.EXECUTABLE_DIRECTORY, "NLog.config"));
#endif

                    var logger = LogManager.GetCurrentClassLogger();

                    Thread.CurrentThread.Name = "Main";

                    logger.Debug("アプリケーションを開始します。");

                    AppDomain.CurrentDomain.UnhandledException += new(CurrentDomain_UnhandledException);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var component = new ComponentManager())
                    using (var initialForm = new InitialForm())
                    {
                        Application.Run(initialForm);
                    }

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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error(ex);
            ExceptionUtil.ShowErrorDialog("予期しないエラーが発生しました。", ex);
        }
    }
}
