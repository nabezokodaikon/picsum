using PicSum.Core.Base.Conf;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows")]
    internal sealed class DirectoryDrawItem
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
                g.FillRectangle(base.Palette.MouseDownBrush, rect);
            }
            else if (base.IsMousePoint)
            {
                g.FillRectangle(base.Palette.MousePointBrush, rect);
                g.DrawRectangle(base.Palette.MousePointPen, rect);
            }

            g.DrawString(this.directory.DirectoryName, base.Palette.TextFont, base.Palette.TextBrush, rect, base.Palette.TextFormat);
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
