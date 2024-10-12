using SWF.Core.ImageAccessor;
using SWF.UIComponent.TabOperation.Properties;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows")]
    internal sealed class TabDropForm
        : Form
    {


        private readonly Bitmap dropMaximumImage = Resources.DropMaximum;
        private readonly Bitmap dropLeftImage = Resources.DropLeft;
        private readonly Bitmap dropRightImage = Resources.DropRight;
        private Bitmap dropImage = null;





        public TabDropForm()
        {
            if (!this.DesignMode)
            {
                this.InitializeComponent();
            }
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
            this.Region = ImageUtil.GetRegion(this.dropMaximumImage, Color.FromArgb(0, 0, 0, 0));
            base.OnLoad(e);
        }





        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = this.dropMaximumImage.Size;
            this.MinimumSize = this.dropMaximumImage.Size;
            this.Size = this.dropMaximumImage.Size;
            this.ShowInTaskbar = false;
            this.Opacity = 0.75;
        }


    }
}
