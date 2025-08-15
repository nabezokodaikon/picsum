using PicSum.Main.Conf;
using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
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
        private static readonly Mutex MUTEX = new(true, AppConstants.MUTEX_NAME);

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Main()
        {
            ConsoleUtil.Write(true, $"Program.Main 1");
            if (MUTEX.WaitOne(TimeSpan.Zero, true))
            {
                ConsoleUtil.Write(true, $"Program.Main 2");
                try
                {
                    AppConstants.SetUIThreadName();
                    ConsoleUtil.Write(true, $"Program.Main 3");

                    TimeMeasuring.Threshold = CommandLineArgs.GetThreshold();
                    ConsoleUtil.Write(true, $"Program.Main 4");

                    WindowsFormsSynchronizationContext.AutoInstall = false;
                    ConsoleUtil.Write(true, $"Program.Main 5");

                    var coreCount = Environment.ProcessorCount;
                    ThreadPool.SetMinThreads(coreCount, coreCount);
                    ConsoleUtil.Write(true, $"Program.Main 6");

                    BootTimeMeasurement.Start();
                    ConsoleUtil.Write(true, $"Program.Main 7");

                    AssemblyPreloader.OptimizeStartup(
                        typeof(Accessibility.AnnoScope),
                        typeof(Microsoft.Win32.SystemEvents),
                        typeof(System.Data.Common.CatalogLocation),
                        typeof(System.Data.SQLite.AssemblySourceIdAttribute),
                        typeof(System.Diagnostics.Process),
                        typeof(System.Diagnostics.TraceSource),
                        typeof(System.IO.DriveInfo),
                        typeof(System.IO.MemoryMappedFiles.MemoryMappedFile),
                        typeof(System.Resources.Extensions.DeserializingResourceReader),
                        typeof(System.Threading.Channels.BoundedChannelFullMode),
                        typeof(System.Threading.ThreadPool),
                        typeof(System.Transactions.CommittableTransaction),
                        typeof(System.Text.RegularExpressions.Capture),
                        typeof(System.Xml.ConformanceLevel),
#if UWP
                        typeof(Windows.Storage.AppDataPaths),
#endif
                        typeof(MemoryPack.BitPackFormatterAttribute),
                        typeof(NLog.Attributes.LogLevelTypeConverter),

                        typeof(HeyRed.ImageSharp.Heif.DecodingMode),
                        typeof(ImageMagick.AlphaOption),
                        typeof(LibHeifSharp.AuxiliaryImageType),
                        typeof(OpenCvSharp.AccessFlag),
                        typeof(OpenCvSharp.Extensions.BitmapConverter),
                        typeof(SixLabors.ImageSharp.Advanced.AdvancedImageExtensions),
                        typeof(Svg.AttributeEventArgs),
                        typeof(ZLinq.DropInGenerateTypes),
                        typeof(ZLinq.FileSystemInfoExtensions),

                        typeof(PicSum.DatabaseAccessor.Connection.FileInfoDB),
                        typeof(PicSum.Job.Common.IThumbnailCacher),
                        typeof(PicSum.UIComponent.AddressBar.AddressBar),
                        typeof(PicSum.UIComponent.Contents.Common.AbstractBrowsePage),
                        typeof(PicSum.UIComponent.InfoPanel.InfoPanel),

                        typeof(SWF.Core.DatabaseAccessor.AbstractDatabase),
                        typeof(SWF.Core.FileAccessor.CacheFileController),
                        typeof(SWF.Core.ImageAccessor.CvImage),
                        typeof(SWF.Core.Job.AbstractAsyncJob),
                        typeof(SWF.Core.ResourceAccessor.ResourceFiles),
                        typeof(SWF.Core.StringAccessor.NaturalStringComparer),

                        typeof(SWF.UIComponent.Base.CheckPatternPanel),
                        typeof(SWF.UIComponent.FlowList.DrawItemChangedEventArgs),
                        typeof(SWF.UIComponent.Form.GrassForm),
                        typeof(SWF.UIComponent.TabOperation.DrawTabEventArgs),
                        typeof(SWF.UIComponent.WideDropDown.AddItemEventArgs),

                        typeof(WinApi.WinApiMembers)
                    );

                    AppFiles.CreateApplicationDirectories();

                    using (TimeMeasuring.Run(true, "Program.Main Load Configs"))
                    {
                        Action[] actions = [
                            () => Log.Initialize(AppFiles.LOG_DIRECTORY.Value),
                            () => Config.INSTANCE.Load()
                        ];

                        Parallel.ForEach(
                            actions,
                            new ParallelOptions { MaxDegreeOfParallelism = actions.Length },
                            _ => _()
                        );
                    }

                    var logger = Log.GetLogger();

                    logger.Info("アプリケーションを開始します。");

#if DEBUG
                    AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
#endif

                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                    Application.ThreadException += Application_ThreadException;
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var context = new Context())
                    {
                        Application.Run(context);
                    }

                    logger.Info("アプリケーションを終了します。\n");
                }
                finally
                {
                    MUTEX.ReleaseMutex();
                }
            }
            else
            {
                var filePath = CommandLineArgs.GetImageFilePathCommandLineArgs();
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                try
                {
                    using (var pipeClient = new NamedPipeClientStream(".", AppConstants.PIPE_NAME, PipeDirection.Out))
                    {
                        pipeClient.Connect(1000);
                        using (var writer = new StreamWriter(pipeClient) { AutoFlush = true })
                        {
                            writer.WriteLine(filePath);
                        }
                    }
                }
                catch (IOException)
                {
                    return;
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Log.GetLogger().Fatal(e.Exception);
            ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.GetLogger().Fatal(ex);
            ExceptionUtil.ShowFatalDialog("Unhandled Non-UI Exception.", ex);
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Log.GetLogger().Trace($"アセンブリが読み込まれました: {args.LoadedAssembly.FullName}");
        }
    }
}
