using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Core;
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
            GC.SuppressFinalize(this);
        }

        public override void Draw(Graphics g)
        {
            ArgumentNullException.ThrowIfNull(g, nameof(g));

            if (this._directory == null)
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

            var scale = WindowUtil.GetCurrentWindowScale(this.AddressBar);
            var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            var text = this._directory.DirectoryName;
            var textSize = TextRenderer.MeasureText(text, font);
            TextRenderer.DrawText(
                g,
                text,
                font,
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
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.OverlapTab, this._directory.DirectoryPath));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                this.OnSelectedDirectory(new SelectedDirectoryEventArgs(PageOpenType.AddTab, this._directory.DirectoryPath));
            }
        }

    }
}
