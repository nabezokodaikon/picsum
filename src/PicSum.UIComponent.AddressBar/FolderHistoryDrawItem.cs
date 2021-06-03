using System;
using System.Drawing;
using System.Windows.Forms;
using PicSum.UIComponent.AddressBar.Properties;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;

namespace PicSum.UIComponent.AddressBar
{
    class FolderHistoryDrawItem : DropDownDrawItemBase, IDisposable
    {
        #region インスタンス変数

        private Image _drawImage = Resources.SmallArrowDown;
        private TwoWayProcess<GetFolderViewHistoryAsyncFacade, ListEntity<FileShallowInfoEntity>> _getFolderHistoryProcess = null;

        #endregion

        #region プロパティ

        private TwoWayProcess<GetFolderViewHistoryAsyncFacade, ListEntity<FileShallowInfoEntity>> getFolderHistoryProcess
        {
            get
            {
                if (_getFolderHistoryProcess == null)
                {
                    _getFolderHistoryProcess = TaskManager.CreateTwoWayProcess<GetFolderViewHistoryAsyncFacade, ListEntity<FileShallowInfoEntity>>(base.components);
                    _getFolderHistoryProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(getFolderHistoryProcess_Callback);
                }

                return _getFolderHistoryProcess;
            }
        }

        #endregion

        #region コンストラクタ

        public FolderHistoryDrawItem()
        {
            initializeComponent();
        }

        #endregion

        #region メソッド

        public new void Dispose()
        {
            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            Rectangle rect = GetRectangle();

            if (base.IsMouseDown || base.IsDropDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
            }

            g.DrawImage(_drawImage, getImageDrawRectangle(_drawImage));
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Button == MouseButtons.Left)
            {
                base.Items.Clear();
                int width = Math.Max(MINIMUM_DROPDOWN_WIDHT, base.AddressBar.Width);
                int height = MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight;
                base.DropDownList.Size = new Size(width, height);
                base.DropDownList.ClearSelectedItems();
                base.DropDownList.ItemCount = 0;
                base.DropDownList.Show(base.AddressBar, 0, base.AddressBar.Height);
                getFolderHistoryProcess.Execute(this);
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
        }

        protected override void drawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(base.DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            FolderEntity item = base.Items[e.ItemIndex];

            if (item.FolderIcon != null)
            {
                int iconSize = Math.Min(base.DropDownList.ItemHeight, item.FolderIcon.Width);

                int iconPoint = (int)((base.DropDownList.ItemHeight - iconSize) / 2);

                Rectangle iconRect = new Rectangle(e.ItemRectangle.X + iconPoint,
                                                   e.ItemRectangle.Y + iconPoint,
                                                   iconSize,
                                                   iconSize);

                e.Graphics.DrawImage(item.FolderIcon, iconRect);
            }

            Rectangle textRect = new Rectangle(e.ItemRectangle.X + base.DropDownList.ItemHeight,
                                               e.ItemRectangle.Y,
                                               e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                                               e.ItemRectangle.Height);

            e.Graphics.DrawString(item.FolderPath,
                                  base.Palette.TextFont,
                                  base.DropDownList.ItemTextBrush,
                                  textRect,
                                  base.DropDownList.ItemTextFormat);
        }

        private void initializeComponent()
        {

        }

        private Rectangle getImageDrawRectangle(Image img)
        {
            int w = img.Width;
            int h = img.Height;
            int x = (int)(base.X + (base.Width - img.Width) / 2d);
            int y = (int)(base.Y + (base.Height - img.Height) / 2d);
            return new Rectangle(x, y, w, h);
        }

        #endregion

        #region イベント

        private void getFolderHistoryProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            int width = 0;

            using (Graphics g = base.DropDownList.CreateGraphics())
            {
                foreach (FileShallowInfoEntity info in e)
                {
                    FolderEntity item = new FolderEntity();
                    item.FolderPath = info.FilePath;
                    item.FolderName = info.FileName;
                    item.FolderIcon = info.SmallIcon;
                    base.Items.Add(item);

                    width = Math.Max(width, (int)g.MeasureString(item.FolderPath + "________", base.Palette.TextFont).Width);
                }
            }

            if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
            {
                width += base.DropDownList.ScrollBarWidth;
            }

            width = Math.Max(width, base.AddressBar.Width);

            int height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                  base.Items.Count * base.DropDownList.ItemHeight);

            base.DropDownList.Size = new Size(width, height);
            base.DropDownList.ItemCount = base.Items.Count;
        }

        #endregion
    }
}
