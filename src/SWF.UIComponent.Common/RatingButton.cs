using System;
using System.Drawing;
using System.Windows.Forms;
using SWF.UIComponent.Common.Properties;

namespace SWF.UIComponent.Common
{
    internal class RatingButton : Control
    {
        private bool _isActive = false;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                this.Invalidate();
            }
        }

        private Image icon
        {
            get
            {
                if (_isActive)
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
            initializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Image icon = this.icon;
            int w = Math.Min(icon.Width, this.Width);
            int h = Math.Min(icon.Height, this.Height);
            int x = (int)((this.Width - icon.Width) / 2f);
            int y = (int)((this.Height - icon.Height) / 2f);
            e.Graphics.DrawImage(icon, x, y, w, h);
        }

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(24, 24);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
        }
    }
}
