using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using SWF.UIComponent.TabOperation.Properties;
using System.Diagnostics;
using SWF.Common;

namespace SWF.UIComponent.TabOperation
{
    class TabDropForm : Form
    {
        #region 定数・列挙

        #endregion

        #region イベント・デリゲート

        #endregion

        #region インスタンス変数

        private Bitmap _dropMaximumImage = Resources.DropMaximum;
        private Bitmap _dropLeftImage = Resources.DropLeft;
        private Bitmap _dropRightImage = Resources.DropRight;
        private Bitmap _dropImage = null;

        #endregion

        #region パブリックプロパティ

        #endregion

        #region 継承プロパティ

        #endregion

        #region プライベートプロパティ

        #endregion

        #region コンストラクタ

        public TabDropForm()
        {
            if (!this.DesignMode)
            {
                initializeComponent();
            }
        }

        #endregion

        #region パブリックメソッド

        public void SetMaximumImage()
        {
            _dropImage = _dropMaximumImage;
        }

        public void SetLeftImage()
        {
            _dropImage = _dropLeftImage;
        }

        public void SetRightImage()
        {
            _dropImage = _dropRightImage;
        }

        #endregion

        #region 継承メソッド

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_dropImage != null)
            {
                e.Graphics.DrawImage(_dropImage, 0, 0, _dropImage.Width, _dropImage.Height);
            }

            base.OnPaint(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Region = ImageUtil.GetRegion(_dropMaximumImage, Color.FromArgb(0, 0, 0, 0));
            base.OnLoad(e);
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = _dropMaximumImage.Size;
            this.MinimumSize = _dropMaximumImage.Size;
            this.Size = _dropMaximumImage.Size;
            this.ShowInTaskbar = false;
            this.Opacity = 0.75;
        }

        #endregion
    }
}
