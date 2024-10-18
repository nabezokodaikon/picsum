using NLog;
using NLog.Config;
using PicSum.Job.Logics;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;
using WinApi;

namespace PicSum.Main
{
    [SupportedOSPlatform("windows")]
    internal sealed class Program
        : MarshalByRefObject
    {
        private static Mutex mutex;

        public static Process GetPreviousProcess()
        {
            var curProcess = Process.GetCurrentProcess();
            var currentPath = curProcess.MainModule.FileName;
            var allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (var checkProcess in allProcesses)
            {
                if (checkProcess.Id != curProcess.Id)
                {
                    string checkPath;

                    try
                    {
                        checkPath = checkProcess.MainModule.FileName;
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        // アクセス権限がない場合は無視
                        continue;
                    }

                    // プロセスのフルパス名を比較して同じアプリケーションか検証
                    if (String.Compare(FileUtil.GetFileName(checkPath), FileUtil.GetFileName(currentPath), true) == 0)
                    {
                        return checkProcess;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            mutex = new Mutex(true, "PicSum", out var createdNew);

            try
            {
                if (!createdNew)
                {
                    var previousProcess = GetPreviousProcess();
                    if (previousProcess != null)
                    {
                        // 既存のプロセスのメインウィンドウを前面に表示
                        var hWnd = previousProcess.MainWindowHandle;
                        if (hWnd != IntPtr.Zero)
                        {
                            WinApiMembers.SetForegroundWindow(hWnd);
                        }
                        else
                        {
                            MessageBox.Show("既に実行中のウィンドウが見つかりませんでした。");
                        }
                    }

                    return;
                }
            }
            finally
            {
                if (createdNew)
                {
                    mutex.ReleaseMutex();
                }
            }

#if DEBUG
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.debug.config");
#elif DEVELOP
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.debug.config");
#else
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
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

            FileIconCash.DisposeStaticResouces();
            ThumbnailGetLogic.DisposeStaticResouces();
            ImageFileCacheUtil.DisposeStaticResouces();
            ImageFileSizeCacheUtil.DisposeStaticResouces();
            FileExportLogic.DisposeStaticResouces();

            logger.Debug("アプリケーションを終了します。");

            return;
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
