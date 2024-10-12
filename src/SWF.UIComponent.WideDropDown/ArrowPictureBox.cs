using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows")]
    public sealed class ArrowPictureBox
        : PictureBox
    {
        private readonly SolidBrush defaultBrush = new(Color.White);

        private readonly SolidBrush enterBrush = new(Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B));

        private readonly SolidBrush selectedBrush = new(Color.FromArgb(
            SystemColors.Highlight.A / 4,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B));

        private bool isMouseEnter = false;
        private bool isSelected = false;

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

            var x = (this.Width - this.Image.Width) / 2f;
            var y = (this.Height - this.Image.Height) / 2f;
            var w = this.Image.Width;
            var h = this.Image.Height;
            pe.Graphics.DrawImage(this.Image, new RectangleF(x, y, w, h));
        }

        private SolidBrush GetBrush()
        {
            if (this.isSelected)
            {
                return this.selectedBrush;
            }
            else if (this.isMouseEnter)
            {
                return this.enterBrush;
            }
            else
            {
                return this.defaultBrush;
            }
        }
    }
}
