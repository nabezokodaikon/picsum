using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    public sealed class ArrowPictureBox
        : PictureBox
    {
        private SolidBrush defaultBrush = new SolidBrush(Color.White);
        private SolidBrush enterBrush = new SolidBrush(Color.White);
        private SolidBrush selectedBrush = new SolidBrush(Color.White);
        private bool isMouseEnter = false;
        private bool isSelected = false;

        public Color DefaultColor
        {
            get
            {
                if (this.defaultBrush != null)
                {
                    return this.defaultBrush.Color;
                }
                else
                {
                    return Color.White;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.defaultBrush.Dispose();
                this.defaultBrush = new SolidBrush(value);
            }
        }

        public Color EnterColor
        {
            get
            {
                if (this.enterBrush != null)
                {
                    return this.enterBrush.Color;
                }
                else
                {
                    return Color.White;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.enterBrush.Dispose();
                this.enterBrush = new SolidBrush(value);
            }
        }

        public Color SelectedColor
        {
            get
            {
                if (this.selectedBrush != null)
                {
                    return this.selectedBrush.Color;
                }
                else
                {
                    return Color.White;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.selectedBrush.Dispose();
                this.selectedBrush = new SolidBrush(value);
            }
        }

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
            }
        }

        public ArrowPictureBox()
        {

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.isMouseEnter = true;
            this.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.isMouseEnter = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            this.isSelected = true;
            this.Invalidate();
            base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.Image == null)
            {
                base.OnPaint(pe);
                return;
            }

            var brush = this.getBrush();
            pe.Graphics.FillRectangle(brush, 0, 0, this.Width, this.Height);

            var x = (this.Width - this.Image.Width) / 2f;
            var y = (this.Height - this.Image.Height) / 2f;
            var w = this.Image.Width;
            var h = this.Image.Height;
            pe.Graphics.DrawImage(this.Image, new RectangleF(x, y, w, h));
        }

        private SolidBrush getBrush()
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
