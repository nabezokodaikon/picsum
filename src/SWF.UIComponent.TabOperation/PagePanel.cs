using SWF.UIComponent.Core;
using System;
using System.ComponentModel;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツコントロール
    /// </summary>
    public partial class PagePanel
        : BaseControl
    {

        public event EventHandler Activated;
        public event EventHandler Inactivated;
        public event EventHandler<DrawTabEventArgs> DrawTabPage;

        private string _title = string.Empty;
        private Image _icon = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

                this._title = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                this._icon = value;
            }
        }

        /// <summary>
        /// コンテンツを再描画します。
        /// </summary>
        public virtual void RedrawPage(float scale)
        {

        }

        public virtual void StopPageDraw()
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

    }
}
