using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Windows.Forms;
using PicSum.Main.Mng;
using PicSum.Main.UIComponent;

namespace PicSum.Main
{
    class Program : MarshalByRefObject
    {
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
                    ChannelServices.RegisterChannel(new IpcServerChannel(Application.ProductName), true);
                    RemotingConfiguration.RegisterWellKnownServiceType(typeof(Program), "Program", WellKnownObjectMode.Singleton);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    using (ComponentManager component = new ComponentManager())
                    {                       
                        _dummyForm = new DummyForm();
                        Application.Run(_dummyForm);
                    }
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
    }
}
