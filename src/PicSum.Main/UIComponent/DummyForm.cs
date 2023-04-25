using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Main.Mng;
using SWF.UIComponent.Common;
using PicSum.Core.Task.AsyncTask;
using System.Windows.Forms;
using System.Drawing;
using SWF.Common;

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

        public void ActivateBrowser()
        {
            BrowserForm browser = _browserManager.GetActiveBrowser();
            browser.Activate();
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
                ExceptionUtil.ShowErrorDialog(e.Task.Exception);
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
