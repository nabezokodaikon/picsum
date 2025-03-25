using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed partial class TabDropForm
        : Form
    {
        private static readonly Color TRANSPARENT_COLOR = Color.FromArgb(0, 0, 0, 0);

        private readonly Bitmap dropMaximumImage = ResourceFiles.DropMaximumIcon.Value;
        private readonly Bitmap dropLeftImage = ResourceFiles.DropLeftIcon.Value;
        private readonly Bitmap dropRightImage = ResourceFiles.DropRightIcon.Value;
        private Bitmap dropImage = null;

        public TabDropForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = this.dropMaximumImage.Size;
            this.MinimumSize = this.dropMaximumImage.Size;
            this.Size = this.dropMaximumImage.Size;
            this.ShowInTaskbar = false;
            this.Opacity = 0.75;
        }

        public void SetMaximumImage()
        {
            this.dropImage = this.dropMaximumImage;
        }

        public void SetLeftImage()
        {
            this.dropImage = this.dropLeftImage;
        }

        public void SetRightImage()
        {
            this.dropImage = this.dropRightImage;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.dropImage != null)
            {
                e.Graphics.DrawImage(this.dropImage, 0, 0, this.dropImage.Width, this.dropImage.Height);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Region = ImageUtil.GetRegion(this.dropMaximumImage, TRANSPARENT_COLOR);
            base.OnLoad(e);
        }
    }
}
