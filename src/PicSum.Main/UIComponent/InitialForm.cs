using PicSum.Core.Task.AsyncTask;
using PicSum.Main.Mng;
using SWF.Common;
using SWF.UIComponent.Common;
using System;
using System.Runtime.Versioning;

namespace PicSum.Main.UIComponent
{
    /// <summary>
    /// ダミーフォーム
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class InitialForm : HideForm
    {
        #region インスタンス変数

        private BrowserManager browserManager = new BrowserManager();

        #endregion

        #region コンストラクタ

        public InitialForm()
        {
            this.InitializeComponent();
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
