using SWF.UIComponent.Common.Properties;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.Common
{
    [SupportedOSPlatform("windows")]
    internal class RatingButton : Control
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
            }
        }

        private Image Icon
        {
            get
            {
                if (this._isActive)
                {
                    return Resources.ActiveRatingIcon;
                }
                else
                {
                    return Resources.InactiveRatingIcon;
                }
            }
        }

        public RatingButton()
        {
            this.InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image icon = this.Icon;
            int w = Math.Min(icon.Width, this.Width);
            int h = Math.Min(icon.Height, this.Height);
            int x = (int)((this.Width - icon.Width) / 2f);
            int y = (int)((this.Height - icon.Height) / 2f);
            e.Graphics.DrawImage(icon, x, y, w, h);
        }

        private void InitializeComponent()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();

            this.Size = new Size(48, 48);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
        }
    }
}
