using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{

    internal sealed partial class DirectoryDrawItem
        : DrawItemBase, IDisposable
    {
        private DirectoryEntity _directory = null;

        public DirectoryEntity Directory
        {
            get
            {
                return this._directory;
            }
            set
            {
                this._directory = value;
            }
        }

        public DirectoryDrawItem()
        {

        }

        public new void Dispose()
        {
            base.Dispose();
            this._disposed = true;
            GC.SuppressFinalize(this);
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._disposed)
            {
                return;
            }

            if (this._directory == null)
            {
                return;
            }

            var rect = this.GetRectangle();

            if (base.IsMouseDown)
            {
                g.FillRectangle(AddressBarResources.MOUSE_POINT_ITEM_BRUSH, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(AddressBarResources.MOUSE_POINT_ITEM_BRUSH, rect);
            }

            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);
            var font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium, scale);
            var text = this._directory.DirectoryName;
            var textSize = TextRenderer.MeasureText(text, font);
            TextRenderer.DrawText(
                g,
                text,
                font,
                new Point(
                    (int)(rect.Location.X + (rect.Width - textSize.Width) / 2f),
                    (int)(rect.Location.Y + (rect.Height - textSize.Height) / 2f)),
                AddressBarResources.ITEM_TEXT_COLOR,
                TextFormatFlags.Top);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {

        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.OverlapTab, this._directory.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenMode.AddTab, this._directory.DirectoryPath));
            }
        }

    }
}
