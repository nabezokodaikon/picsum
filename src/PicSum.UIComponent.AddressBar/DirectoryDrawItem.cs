using SWF.Core.Base;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class DirectoryDrawItem
        : DrawItemBase, IDisposable
    {

        private DirectoryEntity directory = null;

        public DirectoryEntity Directory
        {
            get
            {
                return this.directory;
            }
            set
            {
                this.directory = value;
            }
        }

        public DirectoryDrawItem()
        {

        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this.directory == null)
            {
                return;
            }

            var rect = this.GetRectangle();

            if (base.IsMouseDown)
            {
                g.FillRectangle(Palette.MOUSE_DOWN_BRUSH, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(Palette.MOUSE_POINT_BRUSH, rect);
                g.DrawRectangle(Palette.MOUSE_POINT_PEN, rect);
            }

            var text = this.directory.DirectoryName;
            var textSize = TextRenderer.MeasureText(text, Palette.TEXT_FONT);
            TextRenderer.DrawText(
                g,
                text,
                Palette.TEXT_FONT,
                new Point(
                    (int)(rect.Location.X + (rect.Width - textSize.Width) / 2f),
                    (int)(rect.Location.Y + (rect.Height - textSize.Height) / 2f)),
                Palette.TEXT_BRUSH.Color,
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
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.OverlapTab, this.directory.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.AddTab, this.directory.DirectoryPath));
            }
        }

    }
}
