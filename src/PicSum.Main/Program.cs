using PicSum.Main.Conf;
using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSum.Main
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(true, AppConstants.MUTEX_NAME, out var createdNew))
            {
                ConsoleUtil.Write(true, $"Mutex");
                if (createdNew)
                {
                    BootTimeMeasurement.Start();
                    ConsoleUtil.Write(true, $"BootTimeMeasurement.Start");

                    ProfileOptimization.SetProfileRoot(AppFiles.PROFILE_DIRECTORY.Value);
                    ProfileOptimization.StartProfile(AppFiles.PROFILE_FILE_NAME);
                    ConsoleUtil.Write(true, $"ProfileOptimization.StartProfile");

                    Measuring.SetMeasuringThresholdMilliseconds(CommandLineArgs.GetMeasuringThresholdMilliseconds());
                    ConsoleUtil.Write(true, $"Measuring.SetMeasuringThresholdMilliseconds");

                    var coreCount = Environment.ProcessorCount;
                    ThreadPool.SetMinThreads(coreCount, coreCount);
                    ConsoleUtil.Write(true, $"ThreadPool.SetMinThreads");

                    AppConstants.SetUIThreadName();
                    ConsoleUtil.Write(true, $"AppConstants.SetUIThreadName");

                    WindowsFormsSynchronizationContext.AutoInstall = false;
                    ConsoleUtil.Write(true, $"WindowsFormsSynchronizationContext.AutoInstall");

                    using (Measuring.Time(true, "Program.Main Load Configs"))
                    {
                        Action[] actions = [
                            static () => NLogManager.Initialize(AppFiles.LOG_DIRECTORY.Value),
                            static () => Config.INSTANCE.Load()
                        ];

                        Parallel.ForEach(
                            actions,
                            new ParallelOptions
                            {
                                MaxDegreeOfParallelism = AppConstants.GetHeavyMaxDegreeOfParallelism(actions),
                            },
                            _ => _()
                        );
                    }

                    var logger = NLogManager.GetLogger();
                    logger.Info("アプリケーションを開始します。");

#if UWP
#else
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    Application.ThreadException += Application_ThreadException;
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#endif
                    Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var context = new Context())
                    {
                        Application.Run(context);
                    }

                    logger.Info("アプリケーションを終了します。\n");
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
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            NLogManager.GetLogger().Fatal(e.Exception);
            ExceptionUtil.ShowFatalDialog("Unhandled UI Exception.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            NLogManager.GetLogger().Fatal(ex);
            ExceptionUtil.ShowFatalDialog("Unhandled Non-UI Exception.", ex);
        }

        private static Type[] GetTypes()
        {
            return [
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
                typeof(NLog.GlobalDiagnosticsContext),

                typeof(HeyRed.ImageSharp.Heif.DecodingMode),
                typeof(ImageMagick.AlphaOption),
                typeof(LibHeifSharp.AuxiliaryImageType),
                typeof(OpenCvSharp.AccessFlag),
                typeof(OpenCvSharp.Extensions.BitmapConverter),
                typeof(SixLabors.ImageSharp.Advanced.AdvancedImageExtensions),
                typeof(SkiaSharp.GRContext),
                typeof(Svg.AttributeEventArgs),
                typeof(ZLinq.DropInGenerateTypes),
                typeof(ZLinq.FileSystemInfoExtensions),

                typeof(PicSum.DatabaseAccessor.Connection.FileInfoDao),
                typeof(PicSum.Job.Common.IThumbnailCacher),
                typeof(PicSum.UIComponent.AddressBar.AddressBar),
                typeof(PicSum.UIComponent.Contents.Common.AbstractBrowsePage),
                typeof(PicSum.UIComponent.InfoPanel.InfoPanel),

                typeof(SWF.Core.DatabaseAccessor.AbstractDao),
                typeof(SWF.Core.FileAccessor.CacheFileController),
                typeof(SWF.Core.ImageAccessor.CvImage),
                typeof(SWF.Core.Job.AbstractJob),
                typeof(SWF.Core.ResourceAccessor.ResourceFiles),
                typeof(SWF.Core.StringAccessor.NaturalStringComparer),

                typeof(SWF.UIComponent.Base.CheckPatternPanel),
                typeof(SWF.UIComponent.FlowList.DrawItemChangedEventArgs),
                typeof(SWF.UIComponent.Form.GrassForm),
                typeof(SWF.UIComponent.TabOperation.DrawTabEventArgs),
                typeof(SWF.UIComponent.WideDropDown.AddItemEventArgs),

                typeof(WinApi.WinApiMembers)
            ];
        }
    }
}
