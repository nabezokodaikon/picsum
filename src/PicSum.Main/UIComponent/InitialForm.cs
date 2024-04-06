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

        private BrowserManager browserManager = new();

        #endregion

        #region コンストラクタ

        public InitialForm()
        {

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
