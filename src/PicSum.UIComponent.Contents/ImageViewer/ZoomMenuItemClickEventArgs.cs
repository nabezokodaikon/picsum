using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.UIComponent.Contents.ImageViewer
{
    public sealed class ZoomMenuItemClickEventArgs
    {
        public float ZoomValue { get; private set; }

        public ZoomMenuItemClickEventArgs(float zoomValue)
        {
            this.ZoomValue = zoomValue;
        }
    }
}
