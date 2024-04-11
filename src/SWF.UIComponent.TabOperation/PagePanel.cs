using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツコントロール
    /// </summary>
    public class PagePanel
        : UserControl
    {
        #region イベント・デリゲート

        public event EventHandler Activated;
        public event EventHandler Inactivated;
        public event EventHandler<DrawTabEventArgs> DrawTabPage;

        #endregion

        #region インスタンス変数

        private string title = string.Empty;
        private Image icon = null;

        #endregion

        #region パブリックプロパティ

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                ArgumentNullException.ThrowIfNullOrEmpty(value, nameof(value));

                this.title = value;
            }
        }

        public Image Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
            }
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// コンテンツを再描画します。
        /// </summary>
        public virtual void RedrawPage()
        {

        }

        /// <summary>
        /// アクティブにします。
        /// </summary>
        public void Active()
        {
            this.OnActivated(EventArgs.Empty);
        }

        /// <summary>
        /// 非アクティブにします。
        /// </summary>
        public void Inactive()
        {
            this.OnInactivated(EventArgs.Empty);
        }

        public void DrawingTabPage(DrawTabEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            this.OnDrawTabPage(e);
        }

        #endregion

        #region 継承メソッド

        protected void OnActivated(EventArgs e)
        {
            this.Activated?.Invoke(this, e);
        }

        protected void OnInactivated(EventArgs e)
        {
            this.Inactivated?.Invoke(this, e);
        }

        protected virtual void OnDrawTabPage(DrawTabEventArgs e)
        {
            this.DrawTabPage?.Invoke(this, e);
        }

        #endregion
    }
}
