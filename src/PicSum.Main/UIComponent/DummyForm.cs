using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Main.Mng;
using SWF.UIComponent.Common;
using PicSum.Core.Task.AsyncTask;
using System.Windows.Forms;
using System.Drawing;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ダミーフォーム
    /// </summary>
    class DummyForm : HideForm
    {
        #region インスタンス変数

        private BrowserManager _browserManager = new BrowserManager();

        #endregion

        #region コンストラクタ

        public DummyForm()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void OpenContentsByCommandLineArgs(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            BrowserForm browser = _browserManager.GetActiveBrowser();
            if (args.Length == 1)
            {
                browser.Activate();
            }
            else if (args.Length > 1)
            {
                browser.OpenContentsByCommandLineArgs(args);
            }
            else
            {
                throw new Exception("コマンドライン引数の数が不正です。");
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            TaskManager.TaskStateChanged += new EventHandler<TaskStateChangedEventArgs>(TaskManager_TaskStateChanged);
            _browserManager.BrowserNothing += new EventHandler(_browserManager_BrowserNothing);
        }

        #endregion

        #region タスクマネージャイベント

        private void TaskManager_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (e.Task.IsEnd && e.Task.IsError)
            {
                MessageBox.Show(e.Task.Exception.Message, "PicSum", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ブラウザ管理クラスイベント

        private void _browserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            BrowserForm form = _browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }

        #endregion
    }
}
