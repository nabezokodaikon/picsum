using NLog;
using PicSum.Core.Base.Log;
using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;
using PicSum.Task.AsyncFacade;
using PicSum.Task.AsyncLogic;
using SWF.Common;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PicSum.Main
{
    class Program : MarshalByRefObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static DummyForm _dummyForm = null;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, Application.ProductName))
            {
                if (mutex.WaitOne(0, false))
                {
                    Thread.CurrentThread.Name = "Main";

                    Logger.Info("アプリケーションを開始します。");

                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                    ChannelServices.RegisterChannel(new IpcServerChannel(Application.ProductName), true);
                    RemotingConfiguration.RegisterWellKnownServiceType(typeof(Program), "Program", WellKnownObjectMode.Singleton);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (ComponentManager component = new ComponentManager())
                    {
                        _dummyForm = new DummyForm();
                        Application.Run(_dummyForm);
                    }

                    FileIconCash.DisposeStaticResouces();
                    OperatingKeepListAsyncLogic.DisposeStaticResouces();
                    GetThumbnailAsyncLogic.DisposeStaticResouces();
                    ExportFileAsyncFacade.DisposeStaticResouces();
                    SqlManager.DisposeStaticResouces();
                    LogWriter.DisposeStaticResouces();

                    Logger.Info("アプリケーションを終了します。");
                }
                else
                {
                    ChannelServices.RegisterChannel(new IpcClientChannel(), true);
                    RemotingConfiguration.RegisterWellKnownClientType(typeof(Program), "ipc://" + Application.ProductName + "/Program");
                    Program pg = new Program();
                    pg.OpenContentsByCommandLineArgs(Environment.GetCommandLineArgs());
                }
            }
        }

        public void OpenContentsByCommandLineArgs(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException();
            }

            if (_dummyForm != null && _dummyForm.IsHandleCreated)
            {
                _dummyForm.Invoke((Action)(() =>
                {
                    if (_dummyForm != null && _dummyForm.IsHandleCreated)
                    {
                        _dummyForm.OpenContentsByCommandLineArgs(args);
                    }
                }));
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var exMessage = ExceptionUtil.CreateDetailsMessage(ex);
            var message = string.Format("予期しない例外が発生しました。\n{0}", exMessage);

            Logger.Fatal(message);
            Logger.Fatal("アプリケーションを異常終了します。");

            ExceptionUtil.ShowErrorDialog(message);            
            
            Application.Exit();
        }
    }
}
