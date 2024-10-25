using PicSum.Job.Entities;
using PicSum.Job.Jobs;
using PicSum.UIComponent.AddressBar.Properties;
using SWF.Core.Job;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows")]
    internal sealed partial class SeparatorDrawItem
        : DropDownDrawItemBase, IDisposable
    {
        private bool disposed = false;
        private readonly Image mousePointImage = Resources.SmallArrowRight;
        private readonly Image mouseDownImage = Resources.SmallArrowDown;
        private bool isRead = false;
        private Font selectedSubDirectoryFont = null;
        private TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>> getSubDirectoryJob = null;

        public DirectoryEntity Directory { get; set; }
        public string SelectedSubDirectoryPath { get; set; }

        private TwoWayJob<SubDirectoriesGetJob, ValueParameter<string>, ListResult<FileShallowInfoEntity>> GetSubDirectoryJob
        {
            get
            {
                if (this.getSubDirectoryJob == null)
                {
                    this.getSubDirectoryJob = new();
                    this.getSubDirectoryJob
                        .Callback(_ =>
                        {
                            if (this.disposed)
                            {
                                throw new ObjectDisposedException(this.GetType().FullName);
                            }

                            this.GetSubDirectoryJob_Callback(_);
                        });
                }

                return this.getSubDirectoryJob;
            }
        }

        private Font SelectedSubDirectoryFont
        {
            get
            {
                this.selectedSubDirectoryFont
                    ??= new Font(base.Palette.TextFont.FontFamily,
                                 base.Palette.TextFont.Size,
                                 FontStyle.Bold,
                                 base.Palette.TextFont.Unit,
                                 base.Palette.TextFont.GdiCharSet);

                return this.selectedSubDirectoryFont;
            }
        }

        public SeparatorDrawItem()
        {

        }

        public new void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.selectedSubDirectoryFont?.Dispose();
            this.selectedSubDirectoryFont = null;

            this.getSubDirectoryJob?.Dispose();
            this.getSubDirectoryJob = null;

            this.disposed = true;

            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

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
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                base.DropDownList.Show(base.AddressBar, this.Left, this.Bottom);

                if (!this.isRead)
                {
                    var param = new ValueParameter<string>(this.Directory.DirectoryPath);
                    this.GetSubDirectoryJob.StartJob(param);
                }
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));
        }

        protected override void DrawDropDownItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint)
            {
                e.Graphics.FillRectangle(base.DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            var item = base.Items[e.ItemIndex];

            if (item.DirectoryIcon != null)
            {
                var iconSize = Math.Min(base.DropDownList.ItemHeight, item.DirectoryIcon.Width);

                var iconPoint = (base.DropDownList.ItemHeight - iconSize) / 2f;

                var iconRect = new RectangleF(e.ItemRectangle.X + iconPoint,
                                              e.ItemRectangle.Y + iconPoint,
                                              iconSize,
                                              iconSize);

                e.Graphics.DrawImage(item.DirectoryIcon, iconRect);
            }

            var textRect = new RectangleF(e.ItemRectangle.X + base.DropDownList.ItemHeight,
                                          e.ItemRectangle.Y,
                                          e.ItemRectangle.Width - base.DropDownList.ItemHeight,
                                          e.ItemRectangle.Height);

            e.Graphics.DrawString(item.DirectoryName,
                                  this.GetFont(item.DirectoryPath),
                                  base.DropDownList.ItemTextBrush,
                                  textRect,
                                  base.DropDownList.ItemTextFormat);
        }

        private RectangleF GetImageDrawRectangle(Image img)
        {
            var w = img.Width;
            var h = img.Height;
            var x = base.X + (base.Width - img.Width) / 2f;
            var y = base.Y + (base.Height - img.Height) / 2f;
            return new RectangleF(x, y, w, h);
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

        private void GetSubDirectoryJob_Callback(ListResult<FileShallowInfoEntity> e)
        {
            var width = MINIMUM_DROPDOWN_WIDHT;

            using (var g = base.DropDownList.CreateGraphics())
            {
                var srcItems = e.ToList();
                srcItems.Sort((x, y) => x.FilePath.CompareTo(y.FilePath));
                foreach (var info in srcItems)
                {
                    var item = new DirectoryEntity
                    {
                        DirectoryPath = info.FilePath,
                        DirectoryName = info.FileName,
                        DirectoryIcon = info.SmallIcon
                    };
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

    }
}
