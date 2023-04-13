using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    class DirectoryDrawItem : DrawItemBase, IDisposable
    {
        #region インスタンス変数

        private DirectoryEntity _directory = null;

        #endregion

        #region プロパティ

        public DirectoryEntity Directory
        {
            get
            {
                return _directory;
            }
            set
            {
                _directory = value;
            }
        }

        #endregion

        #region コンストラクタ

        public DirectoryDrawItem()
        {
            initializeComponent();
        }

        #endregion

        #region メソッド

        public new void Dispose()
        {
            base.Dispose();
        }

        public override void  Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (_directory == null)
            {
                return;
            }

            Rectangle rect = GetRectangle();

            if (base.IsMouseDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
            }

            g.DrawString(_directory.DirectoryName, base.Palette.TextFont, base.Palette.TextBrush, rect, base.Palette.TextFormat);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Button == MouseButtons.Left)
            {
                OnSelectedDirectory(new SelectedDirectoryEventArgs(PicSum.Core.Base.Conf.ContentsOpenType.OverlapTab, _directory.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                OnSelectedDirectory(new SelectedDirectoryEventArgs(PicSum.Core.Base.Conf.ContentsOpenType.AddTab, _directory.DirectoryPath));
            }
        }

        private void initializeComponent()
        {

        }

        #endregion
    }
}
