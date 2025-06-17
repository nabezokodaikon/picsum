using NLog;
using PicSum.Main.Conf;
using PicSum.Main.Mng;
using SWF.Core.Base;
using SWF.Core.ConsoleAccessor;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
            ConsoleUtil.Write(true, $"Program.Main");
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    AppConstants.StartBootTimeMeasurement();

                    var coreCount = Environment.ProcessorCount;
                    ThreadPool.SetMinThreads(coreCount * 4, coreCount * 4);

                    AssemblyPreloader.OptimizeStartup(
                        typeof(Accessibility.AnnoScope),
                        typeof(System.AppDomain),
                        typeof(System.Collections.Generic.CollectionExtensions),
                        typeof(System.ComponentModel.AddingNewEventArgs),
                        typeof(System.Data.AcceptRejectRule),
                        typeof(System.Data.SQLite.AssemblySourceIdAttribute),
                        typeof(System.Drawing.Bitmap),
                        typeof(System.IO.BinaryReader),
                        typeof(System.Resources.Extensions.DeserializingResourceReader),
                        typeof(System.Windows.Forms.Application),

#if UWP
                        typeof(Windows.Storage.AppDataPaths),
#endif

                        typeof(MessagePack.CompositeResolverAttribute),
                        typeof(NLog.Attributes.LogLevelTypeConverter),

                        typeof(HeyRed.ImageSharp.Heif.DecodingMode),
                        typeof(ImageMagick.AlphaOption),
                        typeof(LibHeifSharp.AuxiliaryImageType),
                        typeof(OpenCvSharp.AccessFlag),
                        typeof(OpenCvSharp.Extensions.BitmapConverter),
                        typeof(SixLabors.ImageSharp.Advanced.AdvancedImageExtensions),
                        typeof(Svg.AttributeEventArgs),

                        typeof(PicSum.DatabaseAccessor.Connection.FileInfoDB),
                        typeof(PicSum.Job.Common.IThumbnailCacher),
                        typeof(PicSum.UIComponent.AddressBar.AddressBar),
                        typeof(PicSum.UIComponent.Contents.Common.BrowserPage),
                        typeof(PicSum.UIComponent.InfoPanel.InfoPanel),

                        typeof(SWF.Core.DatabaseAccessor.AbstractConnection),
                        typeof(SWF.Core.FileAccessor.FileAppender),
                        typeof(SWF.Core.ImageAccessor.CvImage),
                        typeof(SWF.Core.Job.AbstractAsyncJob),
                        typeof(SWF.Core.ResourceAccessor.ResourceFiles),
                        typeof(SWF.Core.StringAccessor.NaturalStringComparer),

                        typeof(SWF.UIComponent.Core.CheckPatternPanel),
                        typeof(SWF.UIComponent.FlowList.DrawItemChangedEventArgs),
                        typeof(SWF.UIComponent.Form.GrassForm),
                        typeof(SWF.UIComponent.TabOperation.DrawTabEventArgs),
                        typeof(SWF.UIComponent.WideDropDown.AddItemEventArgs),

                        typeof(WinApi.WinApiMembers)
                    );

                    AppConstants.CreateApplicationDirectories();

                    using (TimeMeasuring.Run(true, "Program.Main Load Configs"))
                    {
                        Action[] actions = [
                            () => LogUtil.Initialize(AppConstants.LOG_DIRECTORY.Value),
                            Config.Instance.Load
                        ];

                        Parallel.ForEach(
                            actions,
                            new ParallelOptions { MaxDegreeOfParallelism = actions.Length },
                            _ => _()
                        );
                    }

                    using (ScopeContext.PushProperty(LogUtil.NLOG_PROPERTY, AppConstants.UI_THREAD_NAME))
                    {
                        LogUtil.Logger.Debug("アプリケーションを開始します。");

                        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_OnAssemblyLoad;
                        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                        Application.ThreadException += Application_ThreadException;
                        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        using (var resource = new ResourceManager())
                        using (var context = new Context())
                        {
                            Application.Run(context);
                        }

                        LogUtil.Logger.Debug("アプリケーションを終了します。");
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                var filePath = CommandLineArgs.GetImageFilePathCommandLineArgs();
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

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogUtil.Logger.Fatal(e.Exception);
            ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            LogUtil.Logger.Fatal(ex);
            ExceptionUtil.ShowFatalDialog("Unhandled Non-UI Exception.", ex);
        }

        private static void CurrentDomain_OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
#if DEBUG
            LogUtil.Logger.Trace($"アセンブリが読み込まれました: {args.LoadedAssembly.FullName}");
#endif
        }
    }
}
