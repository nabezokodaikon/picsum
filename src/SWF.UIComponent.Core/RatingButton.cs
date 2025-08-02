using SWF.Core.ResourceAccessor;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal partial class RatingButton
        : BasePaintingControl
    {
        private static readonly Size DEFAULT_SIZE = new(48, 48);

        private float _scale = 1;
        private bool _isActive = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsActive
        {
            get
            {
                return this._isActive;
            }
            set
            {
                this._isActive = value;
            }
        }

        private Image Icon
        {
            get
            {
                if (this._isActive)
                {
                    return ResourceFiles.ActiveRatingIcon.Value;
                }
                else
                {
                    return ResourceFiles.InactiveRatingIcon.Value;
                }
            }
        }

        public RatingButton()
        {
            this.Size = DEFAULT_SIZE;

            this.Paint += this.RatingButton_Paint;
        }

        public void SetControlsBounds(float scale)
        {
            this._scale = scale;
            this.Size = new(
                (int)(DEFAULT_SIZE.Width * scale),
                (int)(DEFAULT_SIZE.Height * scale));
            this.Invalidate();
        }

        private void RatingButton_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            var icon = this.Icon;
            var w = Math.Min(icon.Width * this._scale, this.Width);
            var h = Math.Min(icon.Height * this._scale, this.Height);
            var x = (this.Width - w) / 2f;
            var y = (this.Height - h) / 2f;
            e.Graphics.DrawImage(icon, x, y, w, h);
        }
    }
}
