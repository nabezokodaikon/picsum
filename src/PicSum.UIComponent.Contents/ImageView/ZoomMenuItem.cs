using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageView
{

    public sealed class ZoomMenuItem
        : ToolStripMenuItem
    {
        public new event EventHandler<ZoomMenuItemClickEventArgs> Click;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float ZoomValue { get; private set; }

        public ZoomMenuItem(float zoomValue, string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            this.ZoomValue = zoomValue;
            this.Text = text;

            base.Click += this.ZoomMenuItem_Click;
        }

        private void ZoomMenuItem_Click(object sender, EventArgs e)
        {
            this.Click?.Invoke(this, new ZoomMenuItemClickEventArgs(this.ZoomValue));
        }
    }
}
