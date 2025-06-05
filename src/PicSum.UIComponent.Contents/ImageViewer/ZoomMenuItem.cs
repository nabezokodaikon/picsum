using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration.Pnp;

namespace PicSum.UIComponent.Contents.ImageViewer
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            this.Click?.Invoke(this, new ZoomMenuItemClickEventArgs(this.ZoomValue));
        }
    }
}
