using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツコントロール
    /// </summary>
    public class ContentsPanel : UserControl
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        public event EventHandler Activated;
        public event EventHandler Inactivated;
        public event EventHandler<DrawTabEventArgs> DrawTabContents;

        #endregion

        #region インスタンス変数

        private string _title = string.Empty;
        private Image _icon = null;

        #endregion

        #region パブリックプロパティ

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _title = value;
            }
        }

        public Image Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
            }
        }

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// アクティブにします。
        /// </summary>
        public void Active()
        {
            OnActivated(new EventArgs());
        }

        /// <summary>
        /// 非アクティブにします。
        /// </summary>
        public void Inactive()
        {
            OnInactivated(new EventArgs());
        }

        public void DrawingTabContents(DrawTabEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            OnDrawTabContents(e);
        }

        #endregion

        #region 継承メソッド

        protected void OnActivated(EventArgs e)
        {
            if (Activated != null)
            {
                Activated(this, e);
            }
        }

        protected void OnInactivated(EventArgs e)
        {
            if (Inactivated != null)
            {
                Inactivated(this, e);
            }
        }

        protected virtual void OnDrawTabContents(DrawTabEventArgs e)
        {
            if (DrawTabContents != null)
            {
                DrawTabContents(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {

        }

        #endregion
    }
}
