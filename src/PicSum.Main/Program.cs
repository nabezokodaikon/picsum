using NLog;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using PicSum.Task.AsyncFacade;
using PicSum.Task.AsyncLogic;
using SWF.Common;
using System;
using System.Runtime.Versioning;

// TODO: 多重起動制御処理を、.net8.0に合わせる。
//using System.Runtime.Remoting;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Ipc;

using System.Threading;
using System.Windows.Forms;

namespace PicSum.Main
{
    [SupportedOSPlatform("windows")]
    internal sealed class Program
        : MarshalByRefObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static DummyForm dummyForm = null;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(false, Application.ProductName))
            {
                if (mutex.WaitOne(0, false))
                {
                    Thread.CurrentThread.Name = "Main";

#if DEBUG
                    Logger.Info("アプリケーションを開始します。");
#endif

                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                    // TODO: 多重起動制御処理を、.net8.0に合わせる。
                    //ChannelServices.RegisterChannel(new IpcServerChannel(Application.ProductName), true);
                    //RemotingConfiguration.RegisterWellKnownServiceType(typeof(Program), "Program", WellKnownObjectMode.Singleton);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (var component = new ComponentManager())
                    {
                        Program.dummyForm = new DummyForm();
                        Application.Run(Program.dummyForm);
                    }

                    FileIconCash.DisposeStaticResouces();
                    GetThumbnailAsyncLogic.DisposeStaticResouces();
                    ExportFileAsyncFacade.DisposeStaticResouces();

#if DEBUG
                    Logger.Info("アプリケーションを終了します。");
#endif                
                }
                else
                {
                    // TODO: 多重起動制御処理を、.net8.0に合わせる。
                    //ChannelServices.RegisterChannel(new IpcClientChannel(), true);
                    //RemotingConfiguration.RegisterWellKnownClientType(typeof(Program), "ipc://" + Application.ProductName + "/Program");
                    Program pg = new Program();
                    pg.ActivateBrowser();
                }
            }
        }

        public void ActivateBrowser()
        {
            if (Program.dummyForm != null && Program.dummyForm.IsHandleCreated)
            {
                Program.dummyForm.Invoke((Action)(() =>
                {
                    if (Program.dummyForm != null && Program.dummyForm.IsHandleCreated)
                    {
                        Program.dummyForm.ActivateBrowser();
                    }
                }));
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            ExceptionUtil.ShowErrorDialog("補足されない例外が発生しました。", ex);
        }
    }
}
