using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PicSum.UIComponent.AddressBar.Properties;
using PicSum.Core.Base.Conf;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Common;

namespace PicSum.UIComponent.AddressBar
{
    class SeparatorDrawItem : DropDownDrawItemBase, IDisposable
    {
        #region インスタンス変数

        private Image _mousePointImage = Resources.SmallArrowRight;
        private Image _mouseDownImage = Resources.SmallArrowDown;
        private bool _isRead = false;
        private FolderEntity _folder = null;
        private string _selectedSubFolderPath = string.Empty;
        private Font _selectedSubFolderFont = null;
        private TwoWayProcess<GetSubFoldersAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> _getSubFolderProcess = null;

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

        public string SelectedSubFolderPath
        {
            get
            {
                return _selectedSubFolderPath;
            }
            set
            {
                _selectedSubFolderPath = value;
            }
        }

        private TwoWayProcess<GetSubFoldersAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> getSubFolderProcess
        {
            get
            {
                if (_getSubFolderProcess == null)
                {
                    _getSubFolderProcess = TaskManager.CreateTwoWayProcess<GetSubFoldersAsyncFacade, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(base.components);
                    _getSubFolderProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(getSubFolderProcess_Callback);
                }

                return _getSubFolderProcess;
            }
        }

        private Font selectedSubFolderFont
        {
            get
            {
                if (_selectedSubFolderFont == null)
                {
                    _selectedSubFolderFont = new Font(base.Palette.TextFont.FontFamily,
                                                      base.Palette.TextFont.Size,
                                                      FontStyle.Bold,
                                                      base.Palette.TextFont.Unit,
                                                      base.Palette.TextFont.GdiCharSet);
                }

                return _selectedSubFolderFont;
            }
        }

        #endregion

        #region コンストラクタ

        public SeparatorDrawItem()
        {
            initializeComponent();
        }

        #endregion

        #region メソッド

        public new void Dispose()
        {
            if (_selectedSubFolderFont != null)
            {
                _selectedSubFolderFont.Dispose();
            }

            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (base.Palette == null)
            {
                return;
            }

            Rectangle rect = GetRectangle();

            if (base.IsMouseDown || base.IsDropDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
                g.DrawImage(_mouseDownImage, getImageDrawRectangle(_mouseDownImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
                g.DrawImage(_mousePointImage, getImageDrawRectangle(_mousePointImage));
            }
            else
            {
                g.DrawImage(_mousePointImage, getImageDrawRectangle(_mousePointImage));
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (e.Button == MouseButtons.Left)
            {
                base.DropDownList.Show(base.AddressBar, this.Left, this.Bottom);

                if (!_isRead)
                {
                    SingleValueEntity<string> param = new SingleValueEntity<string>();
                    param.Value = _folder.FolderPath;
                    getSubFolderProcess.Execute(this, param);
                }
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

            e.Graphics.DrawString(item.FolderName,
                                  getFont(item.FolderPath),
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

        private Font getFont(string folderPath)
        {
            if (folderPath.Equals(_selectedSubFolderPath, StringComparison.Ordinal))
            {
                return selectedSubFolderFont;
            }
            else
            {
                return base.Palette.TextFont;
            }
        }

        #endregion

        #region イベント

        private void getSubFolderProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            int width = MINIMUM_DROPDOWN_WIDHT;

            using (Graphics g = base.DropDownList.CreateGraphics())
            {
                List<FileShallowInfoEntity> srcItems = e.ToList();
                srcItems.Sort((x, y) => x.FilePath.CompareTo(y.FilePath));
                foreach (FileShallowInfoEntity info in srcItems )
                {
                    FolderEntity item = new FolderEntity();
                    item.FolderPath = info.FilePath;
                    item.FolderName = info.FileName;
                    item.FolderIcon = info.SmallIcon;
                    base.Items.Add(item);

                    width = Math.Max(width, (int)g.MeasureString(item.FolderName + "________", selectedSubFolderFont).Width);
                }
            }

            if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
            {
                width += base.DropDownList.ScrollBarWidth;
            }

            int height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                  base.Items.Count * base.DropDownList.ItemHeight);

          
            base.DropDownList.Size = new Size(width + base.DropDownList.ItemHeight, height);
            base.DropDownList.ItemCount = base.Items.Count;            

            FolderEntity selectedItem = base.Items.SingleOrDefault(item => item.FolderPath.Equals(_selectedSubFolderPath, StringComparison.Ordinal));
            if (selectedItem != null)
            {
                base.DropDownList.SelectItem(base.Items.IndexOf(selectedItem));
            }

            _isRead = true;
        }

        #endregion
    }
}
