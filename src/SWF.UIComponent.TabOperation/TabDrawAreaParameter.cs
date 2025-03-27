using SWF.Core.Base;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal sealed class TabDrawAreaParameter
    {
        private const float HEIGHT = 29;
        private const float TAB_WIDTH = 256;
        private const float SIDE_WIDTH = 8;
        private const float PAGE_SIZE = 24;
        private const float PAGE_OFFSET = 2;

        public float Height { get; private set; }
        public float TabWidth { get; private set; }
        public float SideWidth { get; private set; }
        public float PageSize { get; private set; }
        public float PageOffset { get; private set; }
        public RectangleF IconRectangle { get; private set; }
        public RectangleF CloseButtonRectangle { get; private set; }

        public TabDrawAreaParameter(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            var scale = AppConstants.GetCurrentWindowScale(owner.Handle);
            this.SetPrameters(scale);
        }

        public void Update(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            var scale = AppConstants.GetCurrentWindowScale(owner.Handle);
            this.SetPrameters(scale);
        }

        private void SetPrameters(float scale)
        {
            this.Height = HEIGHT * scale;
            this.TabWidth = TAB_WIDTH * scale;
            this.SideWidth = SIDE_WIDTH * scale;
            this.PageSize = PAGE_SIZE * scale;
            this.PageOffset = PAGE_OFFSET * scale;

            this.IconRectangle = new(
                this.SideWidth, this.PageOffset, this.PageSize, this.PageSize);

            this.CloseButtonRectangle = new(
                this.TabWidth - this.SideWidth - this.PageSize,
                this.PageOffset,
                this.PageSize,
                this.PageSize);
        }
    }
}
