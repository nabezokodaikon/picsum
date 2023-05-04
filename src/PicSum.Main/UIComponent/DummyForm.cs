using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Mng;
using SWF.Common;
using SWF.UIComponent.Common;
using System;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ダミーフォーム
    /// </summary>
    internal sealed class DummyForm : HideForm
    {
        #region インスタンス変数

        private BrowserManager browserManager = new BrowserManager();

        #endregion

        #region コンストラクタ

        public DummyForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region パブリックメソッド

        public void ActivateBrowser()
        {
            var browser = this.browserManager.GetActiveBrowser();
            browser.Activate();
        }

        #endregion

        #region プライベートメソッド

        private void InitializeComponent()
        {
            TaskManager.TaskStateChanged += new EventHandler<TaskStateChangedEventArgs>(this.TaskManager_TaskStateChanged);
            this.browserManager.BrowserNothing += new EventHandler(this.BrowserManager_BrowserNothing);
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

        private void BrowserManager_BrowserNothing(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 継承メソッド

        protected override void OnLoad(EventArgs e)
        {
            var form = this.browserManager.GetActiveBrowser();
            form.Show();

            base.OnLoad(e);
        }

        #endregion
    }
}
