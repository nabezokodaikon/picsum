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


        public event EventHandler Activated;
        public event EventHandler Inactivated;
        public event EventHandler<DrawTabEventArgs> DrawTabPage;





        private string title = string.Empty;
        private Image icon = null;





        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

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
