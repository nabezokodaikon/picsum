using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SWF.UIComponent.Base
{

    public partial class BaseButton
        : Button
    {
        private static readonly SolidBrush DEFAULT_BRUSH = new(Color.FromArgb(250, 250, 250));
        private static readonly SolidBrush MOUSE_POINT_BRUSH = new(Color.FromArgb(220, 220, 220));
        private static readonly SolidBrush DISABLED_BRUSH = new(Color.FromArgb(255, 250, 250, 250));
        private static readonly float[][] MATRIX_ITEMS = [
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, 0.75f, 0],
            [0, 0, 0, 0, 1]
        ];
        private static readonly ColorMatrix COLOR_MATRIX = new(MATRIX_ITEMS);
        private static readonly ImageAttributes IMG_ATTR = CreateImageAttributes();

        public static ImageAttributes CreateImageAttributes()
        {
            var imgAttr = new ImageAttributes();
            imgAttr.SetColorMatrix(
                COLOR_MATRIX, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            return imgAttr;
        }

        private bool _isLeftClick = false;
        private bool _isMousePoint = false;

        public new bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            private set
            {
                base.AutoSize = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            private set
            {
                base.BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            private set
            {
                base.DoubleBuffered = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Font Font
        {
            get
            {
                return base.Font;
            }
            private set
            {
                base.Font = value;
            }
        }

        public new string Name
        {
            get
            {
                return base.Name;
            }
            private set
            {
                base.Name = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            private set
            {
                base.TabIndex = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            private set
            {
                base.TabStop = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SolidBrush DefaultBrush { get; set; } = DEFAULT_BRUSH;

        public BaseButton()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl,
                false);
            this.UpdateStyles();

            this.DoubleBuffered = true;

            this.AutoSize = false;
            this.TabIndex = 0;
            this.TabStop = false;
            this.BackColor = Color.Transparent;

            this.MouseEnter += this.BaseIconButton_MouseEnter;
            this.MouseLeave += this.BaseIconButton_MouseLeave;
            this.Paint += this.BaseButton_Paint;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (e.Button == MouseButtons.Left)
            {
                this._isLeftClick = !this._isLeftClick;
                if (this._isLeftClick)
                {
                    base.OnMouseClick(e);
                }
            }
            else
            {
                base.OnMouseClick(e);
            }
        }

        protected virtual void Draw(PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BaseIconButton_MouseEnter(object? sender, EventArgs e)
        {
            this._isMousePoint = true;
        }

        private void BaseIconButton_MouseLeave(object? sender, EventArgs e)
        {
            this._isMousePoint = false;
        }

        private void BaseButton_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

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

            this.Draw(e);

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
