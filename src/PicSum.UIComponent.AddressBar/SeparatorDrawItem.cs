using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncTask;
using PicSum.Task.Entity;
using PicSum.UIComponent.AddressBar.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows")]
    internal sealed class SeparatorDrawItem
        : DropDownDrawItemBase, IDisposable
    {
        #region インスタンス変数

        private Image mousePointImage = Resources.SmallArrowRight;
        private Image mouseDownImage = Resources.SmallArrowDown;
        private bool isRead = false;
        private Font selectedSubDirectoryFont = null;
        private TwoWayProcess<GetSubDirectoryAsyncTask, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> getSubDirectoryProcess = null;

        #endregion

        #region プロパティ

        public DirectoryEntity Directory { get; set; }
        public string SelectedSubDirectoryPath { get; set; }

        private TwoWayProcess<GetSubDirectoryAsyncTask, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>> GetSubDirectoryProcess
        {
            get
            {
                if (this.getSubDirectoryProcess == null)
                {
                    this.getSubDirectoryProcess = TaskManager.CreateTwoWayProcess<GetSubDirectoryAsyncTask, SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>(base.Components);
                    this.getSubDirectoryProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<FileShallowInfoEntity>>(this.GetSubDirectoryProcess_Callback);
                }

                return this.getSubDirectoryProcess;
            }
        }

        private Font SelectedSubDirectoryFont
        {
            get
            {
                if (this.selectedSubDirectoryFont == null)
                {
                    this.selectedSubDirectoryFont = new Font(base.Palette.TextFont.FontFamily,
                                                              base.Palette.TextFont.Size,
                                                              FontStyle.Bold,
                                                              base.Palette.TextFont.Unit,
                                                              base.Palette.TextFont.GdiCharSet);
                }

                return this.selectedSubDirectoryFont;
            }
        }

        #endregion

        #region コンストラクタ

        public SeparatorDrawItem()
        {

        }

        #endregion

        #region メソッド

        public new void Dispose()
        {
            if (this.selectedSubDirectoryFont != null)
            {
                this.selectedSubDirectoryFont.Dispose();
            }

            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException(nameof(g));
            }

            if (base.Palette == null)
            {
                return;
            }

            var rect = this.GetRectangle();

            if (base.IsMouseDown || base.IsDropDown)
            {
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
                g.DrawRectangle(base.Palette.MousePointPen, rect);
                g.DrawImage(this.mouseDownImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
                g.DrawRectangle(base.Palette.MousePointPen, rect);
                g.DrawImage(this.mousePointImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
            else
            {
                g.DrawImage(this.mousePointImage, this.GetImageDrawRectangle(this.mousePointImage));
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.Button == MouseButtons.Left)
            {
                base.DropDownList.Show(base.AddressBar, this.Left, this.Bottom);

                if (!this.isRead)
                {
                    SingleValueEntity<string> param = new SingleValueEntity<string>();
                    param.Value = this.Directory.DirectoryPath;
                    this.GetSubDirectoryProcess.Execute(this, param);
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

        protected override void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint)
            {
                e.Graphics.FillRectangle(base.DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            if (e.IsSelected)
            {
                e.Graphics.DrawRectangle(base.DropDownList.SelectedItemPen, e.ItemRectangle);
            }

            var item = base.Items[e.ItemIndex];

            if (item.DirectoryIcon != null)
            {
                var iconSize = Math.Min(base.DropDownList.ItemHeight, item.DirectoryIcon.Width);

                var iconPoint = (int)((base.DropDownList.ItemHeight - iconSize) / 2);

                var iconRect = new Rectangle(e.ItemRectangle.X + iconPoint,
                                             e.ItemRectangle.Y + iconPoint,
                                             iconSize,
                                             iconSize);

                e.Graphics.DrawImage(item.DirectoryIcon, iconRect);
            }

            var textRect = new Rectangle(e.ItemRectangle.X + base.DropDownList.ItemHeight,
                                         e.ItemRectangle.Y,
                                         e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                                         e.ItemRectangle.Height);

            e.Graphics.DrawString(item.DirectoryName,
                                  this.GetFont(item.DirectoryPath),
                                  base.DropDownList.ItemTextBrush,
                                  textRect,
                                  base.DropDownList.ItemTextFormat);
        }

        private Rectangle GetImageDrawRectangle(Image img)
        {
            var w = img.Width;
            var h = img.Height;
            var x = (int)(base.X + (base.Width - img.Width) / 2d);
            var y = (int)(base.Y + (base.Height - img.Height) / 2d);
            return new Rectangle(x, y, w, h);
        }

        private Font GetFont(string directoryPath)
        {
            if (directoryPath.Equals(this.SelectedSubDirectoryPath, StringComparison.Ordinal))
            {
                return this.SelectedSubDirectoryFont;
            }
            else
            {
                return base.Palette.TextFont;
            }
        }

        #endregion

        #region イベント

        private void GetSubDirectoryProcess_Callback(object sender, ListEntity<FileShallowInfoEntity> e)
        {
            var width = MINIMUM_DROPDOWN_WIDHT;

            using (var g = base.DropDownList.CreateGraphics())
            {
                var srcItems = e.ToList();
                srcItems.Sort((x, y) => x.FilePath.CompareTo(y.FilePath));
                foreach (var info in srcItems)
                {
                    var item = new DirectoryEntity();
                    item.DirectoryPath = info.FilePath;
                    item.DirectoryName = info.FileName;
                    item.DirectoryIcon = info.SmallIcon;
                    base.Items.Add(item);

                    width = Math.Max(width, (int)g.MeasureString(item.DirectoryName + "________", this.SelectedSubDirectoryFont).Width);
                }
            }

            if (base.Items.Count > MAXIMUM_SHOW_ITEM_COUNT)
            {
                width += base.DropDownList.ScrollBarWidth;
            }

            var height = Math.Min(MAXIMUM_SHOW_ITEM_COUNT * base.DropDownList.ItemHeight,
                                  base.Items.Count * base.DropDownList.ItemHeight);


            base.DropDownList.Size = new Size(width + base.DropDownList.ItemHeight, height);
            base.DropDownList.ItemCount = base.Items.Count;

            var selectedItem = base.Items.SingleOrDefault(item => item.DirectoryPath.Equals(this.SelectedSubDirectoryPath, StringComparison.Ordinal));
            if (selectedItem != null)
            {
                base.DropDownList.SelectItem(base.Items.IndexOf(selectedItem));
            }

            this.isRead = true;
        }

        #endregion
    }
}
