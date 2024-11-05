using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal partial class RatingButton : Control
    {
        private bool _isActive = false;

        public bool IsActive
        {
            get
            {
                return this._isActive;
            }
            set
            {
                this._isActive = value;
                this.Invalidate();
                this.Update();
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
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.Size = new Size(48, 48);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var icon = this.Icon;
            var w = Math.Min(icon.Width, this.Width);
            var h = Math.Min(icon.Height, this.Height);
            var x = (this.Width - icon.Width) / 2f;
            var y = (this.Height - icon.Height) / 2f;
            e.Graphics.DrawImage(icon, x, y, w, h);
        }
    }
}
