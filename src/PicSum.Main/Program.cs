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
                    AppConstants.SetUIThreadName();
                    ConsoleUtil.Write(true, $"AppConstants.SetUIThreadName");

                    BootTimeMeasurement.Start();
                    ConsoleUtil.Write(true, $"BootTimeMeasurement.Start");

                    ProfileOptimization.SetProfileRoot(AppFiles.PROFILE_DIRECTORY.Value);
                    ProfileOptimization.StartProfile(AppFiles.PROFILE_FILE_NAME);
                    ConsoleUtil.Write(true, $"ProfileOptimization.StartProfile");

                    //AssemblyPreloader.OptimizeStartup(GetAssemblyNames());
                    //ConsoleUtil.Write(true, $"AssemblyPreloader.OptimizeStartup");

                    Measuring.SetMeasuringThresholdMilliseconds(CommandLineArgs.GetMeasuringThresholdMilliseconds());
                    ConsoleUtil.Write(true, $"Measuring.SetMeasuringThresholdMilliseconds");

                    var coreCount = Environment.ProcessorCount;
                    ThreadPool.SetMinThreads(coreCount, coreCount);
                    ConsoleUtil.Write(true, $"ThreadPool.SetMinThreads");

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

        private static string[] GetAssemblyNames()
        {
            return [
                ////"aom",
                //"ExCSS",
                //"ImageSharp.Heif",
                ////"libde265",
                ////"libheif",
                //"LibHeifSharp",
                ////"libSkiaSharp",
                ////"libx265",
                ////"Magick.Native-Q8-x64",
                //"Magick.NET.Core",
                //"Magick.NET.SystemDrawing",
                //"Magick.NET-Q8-x64",
                //"MemoryPack.Core",
                //"MetadataExtractor",
                //"Microsoft.Windows.SDK.NET",
                //"NLog",
                ////"opencv_videoio_ffmpeg4110_64",
                //"OpenCvSharp",
                //"OpenCvSharp.Extensions",
                ////"OpenCvSharpExtern",
                //"OpenTK",
                //"OpenTK.GLControl",
                //"SixLabors.ImageSharp",
                //"SkiaSharp",
                //"SkiaSharp.Views.Desktop.Common",
                //"SkiaSharp.Views.WindowsForms",
                ////"SQLite.Interop",
                //"Svg",
                //"System.Data.SQLite",
                //"WinRT.Runtime",
                //"XmpCore",
                //"ZLinq",
                //"ZLinq.FileSystem",

                "PicSum.DatabaseAccessor",
                "PicSum.Job",
                "PicSum.UIComponent.AddressBar",
                "PicSum.UIComponent.Contents",
                "PicSum.UIComponent.InfoPanel",

                //"SWF.Core.DatabaseAccessor",
                //"SWF.Core.FileAccessor",
                //"SWF.Core.ImageAccessor",
                //"SWF.Core.Job",
                //"SWF.Core.ResourceAccessor",
                //"SWF.Core.StringAccessor",

                //"SWF.UIComponent.Base",
                //"SWF.UIComponent.FlowList",
                //"SWF.UIComponent.Form",
                //"SWF.UIComponent.SKFlowList",
                //"SWF.UIComponent.TabOperation",
                //"SWF.UIComponent.WideDropDown",

                //"WinApi"
            ];
        }
    }
}
