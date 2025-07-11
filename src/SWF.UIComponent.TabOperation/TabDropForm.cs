using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
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

        private readonly Bitmap _dropLeftImage = ResourceFiles.DropLeftIcon.Value;
        private readonly Bitmap _dropRightImage = ResourceFiles.DropRightIcon.Value;
        private Bitmap _dropImage = null;

        public TabDropForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = this._dropLeftImage.Size;
            this.MinimumSize = this.MaximumSize;
            this.Size = this.MaximumSize;
            this.ShowInTaskbar = false;
            this.Opacity = 0.75;
            this.Region = ImageUtil.GetRegion(this._dropLeftImage, TRANSPARENT_COLOR);

            this.Paint += this.TabDropForm_Paint;
        }

        public void SetLeftImage()
        {
            this._dropImage = this._dropLeftImage;
        }

        public void SetRightImage()
        {
            this._dropImage = this._dropRightImage;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        private void TabDropForm_Paint(object sender, PaintEventArgs e)
        {
            if (this._dropImage != null)
            {
                e.Graphics.DrawImage(this._dropImage, 0, 0, this._dropImage.Width, this._dropImage.Height);
            }
        }
    }
}
