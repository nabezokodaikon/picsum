using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    class FolderDrawItem : DrawItemBase, IDisposable
    {
        #region インスタンス変数

        private FolderEntity _folder = null;

        #endregion

        #region プロパティ

        public FolderEntity Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
            }
        }

        #endregion

        #region コンストラクタ

        public FolderDrawItem()
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

            if (_folder == null)
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

            g.DrawString(_folder.FolderName, base.Palette.TextFont, base.Palette.TextBrush, rect, base.Palette.TextFormat);
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
                OnSelectedFolder(new SelectedFolderEventArgs(PicSum.Core.Base.Conf.ContentsOpenType.OverlapTab, _folder.FolderPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                OnSelectedFolder(new SelectedFolderEventArgs(PicSum.Core.Base.Conf.ContentsOpenType.AddTab, _folder.FolderPath));
            }
        }

        private void initializeComponent()
        {

        }

        #endregion
    }
}
