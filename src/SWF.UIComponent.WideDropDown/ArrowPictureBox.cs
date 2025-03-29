using SWF.Core.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class ArrowPictureBox
        : PictureBox
    {
        private static readonly SolidBrush DEFAULT_BRUSH = new(Color.White);

        private static readonly SolidBrush ENTER_BRUSH = new(Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B));

        private static readonly SolidBrush SELECTED_BRUSH = new(Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B));

        private bool isMouseEnter = false;
        private bool isSelected = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
                this.Invalidate();
                this.Update();
            }
        }

        public ArrowPictureBox()
        {

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.isMouseEnter = true;
            this.Invalidate();
            this.Update();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.isMouseEnter = false;
            this.Invalidate();
            this.Update();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.isSelected = true;
            this.Invalidate();
            this.Update();
            base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.Image == null)
            {
                base.OnPaint(pe);
                return;
            }

            var brush = this.GetBrush();
            pe.Graphics.FillRectangle(brush, 0, 1, this.Width, this.Height - 2);

            var scale = AppConstants.GetCurrentWindowScale(this.Handle);
            var margin = 12 * scale;
            var size = Math.Min(this.Width, this.Height);
            var w = Math.Min(this.Image.Width * scale, size) - margin;
            var h = Math.Min(this.Image.Height * scale, size) - margin;
            var x = (this.Width - w) / 2f;
            var y = (this.Height - h) / 2f;

            pe.Graphics.DrawImage(this.Image, new RectangleF(x, y, w, h));
        }

        private SolidBrush GetBrush()
        {
            if (this.isSelected)
            {
                return SELECTED_BRUSH;
            }
            else if (this.isMouseEnter)
            {
                return ENTER_BRUSH;
            }
            else
            {
                return DEFAULT_BRUSH;
            }
        }
    }
}
