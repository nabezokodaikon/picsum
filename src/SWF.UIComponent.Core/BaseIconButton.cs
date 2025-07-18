using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseIconButton
        : BaseButton
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

        private bool _isMousePoint = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SolidBrush DefaultBrush { get; set; } = DEFAULT_BRUSH;

        public BaseIconButton()
        {
            this.MouseEnter += this.ToolIconButton_MouseEnter;
            this.MouseLeave += this.ToolIconButton_MouseLeave;
            this.Paint += this.ToolIconButton_Paint;
        }

        private void ToolIconButton_MouseEnter(object? sender, EventArgs e)
        {
            this._isMousePoint = true;
        }

        private void ToolIconButton_MouseLeave(object? sender, EventArgs e)
        {
            this._isMousePoint = false;
        }

        private void ToolIconButton_Paint(object? sender, PaintEventArgs e)
        {
            if (this._isMousePoint)
            {
                e.Graphics.FillRectangle(
                    MOUSE_POINT_BRUSH, 0, 0, this.Width, this.Height);
            }
            else
            {
                e.Graphics.FillRectangle(
                    this.DefaultBrush, 0, 0, this.Width, this.Height);
            }

            if (this.Image != null)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

                var scale = WindowUtil.GetCurrentWindowScale(this);
                var length = Math.Min(this.Width - 12 * scale, this.Height - 12 * scale);
                var w = length;
                var h = length;
                var x = (this.Width - w) / 2f;
                var y = (this.Height - h) / 2f;

                e.Graphics.DrawImage(this.Image, x, y, w, h);

                if (!this.Enabled)
                {
                    using (var overlay = new Bitmap(this.ClientSize.Width, this.ClientSize.Height))
                    using (var overlayGraphics = Graphics.FromImage(overlay))
                    {
                        overlayGraphics.FillRectangle(
                            DISABLED_BRUSH,
                            new Rectangle(0, 0, overlay.Width, overlay.Height));
                        e.Graphics.DrawImage(
                            overlay,
                            new Rectangle(0, 0, overlay.Width, overlay.Height),
                            0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel, IMG_ATTR);
                    }
                }
            }
        }
    }
}
