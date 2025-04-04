using SWF.Core.Base;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class ToolIconButton
        : ToolButton
    {
        private static readonly SolidBrush DEFAULT_BRUSH = new(Color.FromArgb(250, 250, 250));
        private static readonly SolidBrush MOUSE_POINT_BRUSH = new(Color.FromArgb(220, 220, 220));
        private static readonly SolidBrush DISABLED_BRUSH = new(Color.FromArgb(255, 250, 250, 250));
        private static readonly float[][] MATRIX_ITEMS = {
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, 0.75f, 0],
            [0, 0, 0, 0, 1]
        };
        private static readonly ColorMatrix COLOR_MATRIX = new(MATRIX_ITEMS);
        private static readonly ImageAttributes IMG_ATTR = CreateImageAttributes();

        public static ImageAttributes CreateImageAttributes()
        {
            var imgAttr = new ImageAttributes();
            imgAttr.SetColorMatrix(
                COLOR_MATRIX, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            return imgAttr;
        }

        private bool isMousePoint = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SolidBrush DefaultBrush { get; set; } = DEFAULT_BRUSH;

        protected override void OnMouseEnter(EventArgs e)
        {
            this.isMousePoint = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.isMousePoint = false;
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (this.isMousePoint)
            {
                pevent.Graphics.FillRectangle(
                    MOUSE_POINT_BRUSH, 0, 0, this.Width, this.Height);
            }
            else
            {
                pevent.Graphics.FillRectangle(
                    this.DefaultBrush, 0, 0, this.Width, this.Height);
            }

            if (this.Image != null)
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                pevent.Graphics.CompositingQuality = CompositingQuality.HighQuality;

                var scale = AppConstants.GetCurrentWindowScale(this);
                var length = Math.Min(this.Width - 12 * scale, this.Height - 12 * scale);
                var w = length;
                var h = length;
                var x = (this.Width - w) / 2f;
                var y = (this.Height - h) / 2f;

                pevent.Graphics.DrawImage(this.Image, x, y, w, h);

                if (!this.Enabled)
                {
                    using (var overlay = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
                    using (var overlayGraphics = Graphics.FromImage(overlay))
                    {
                        overlayGraphics.FillRectangle(
                            DISABLED_BRUSH,
                            new Rectangle(0, 0, overlay.Width, overlay.Height));
                        pevent.Graphics.DrawImage(
                            overlay,
                            new Rectangle(0, 0, overlay.Width, overlay.Height),
                            0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel, IMG_ATTR);
                    }
                }
            }

            //base.OnPaint(pevent);
        }
    }
}
