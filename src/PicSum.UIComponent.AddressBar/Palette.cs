using System.Drawing;
using System.Runtime.Versioning;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows")]
    internal sealed class Palette
    {
        private Font textFont = new("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));

        private Color textColor = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);

        private Color mousePointColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private Color selectedColor = Color.FromArgb(
            SystemColors.Highlight.A / 8,
            SystemColors.Highlight.R,
            SystemColors.Highlight.G,
            SystemColors.Highlight.B);

        private Color outlineColor = SystemColors.ControlDark;

        private Color innerColor = Color.White;
        private StringTrimming textTrimming = StringTrimming.EllipsisCharacter;
        private StringAlignment textAlignment = StringAlignment.Center;
        private StringAlignment textLineAlignment = StringAlignment.Center;
        private StringFormatFlags textFormatFlags = 0;
        private SolidBrush textBrush = null;
        private SolidBrush mousePointBrush = null;
        private Pen mousePointPen = null;
        private SolidBrush mouseDownBrush = null;
        private SolidBrush outlineBrush = null;
        private SolidBrush innerBrush = null;
        private StringFormat textFormat = null;

        public Font TextFont
        {
            get
            {
                return this.textFont;
            }
            set
            {
                this.textFont = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return this.textColor;
            }
            set
            {
                this.textColor = value;
            }
        }

        public Color MousePointColor
        {
            get
            {
                return this.mousePointColor;
            }
            set
            {
                this.mousePointColor = value;
            }
        }

        public Color MouseDownColor
        {
            get
            {
                return this.selectedColor;
            }
            set
            {
                this.selectedColor = value;
            }
        }

        public Color OutlineColor
        {
            get
            {
                return this.outlineColor;
            }
            set
            {
                this.outlineColor = value;
            }
        }

        public Color InnerColor
        {
            get
            {
                return this.innerColor;
            }
            set
            {
                this.innerColor = value;
            }
        }

        public StringTrimming TextTrimming
        {
            get
            {
                return this.textTrimming;
            }
            set
            {
                this.textTrimming = value;
            }
        }

        public StringAlignment TextAlignment
        {
            get
            {
                return this.textAlignment;
            }
            set
            {
                this.textAlignment = value;
            }
        }

        public StringAlignment TextLineAlignment
        {
            get
            {
                return this.textLineAlignment;
            }
            set
            {
                this.textLineAlignment = value;
            }
        }

        public StringFormatFlags TextFormatFlags
        {
            get
            {
                return this.textFormatFlags;
            }
            set
            {
                this.textFormatFlags = value;
            }
        }

        public SolidBrush TextBrush
        {
            get
            {
                this.textBrush ??= new SolidBrush(this.textColor);
                return this.textBrush;
            }
        }

        public SolidBrush MousePointBrush
        {
            get
            {
                this.mousePointBrush ??= new SolidBrush(this.mousePointColor);
                return this.mousePointBrush;
            }
        }

        public Pen MousePointPen
        {
            get
            {
                this.mousePointPen ??= new Pen(this.mousePointColor);
                return this.mousePointPen;
            }
        }

        public SolidBrush MouseDownBrush
        {
            get
            {
                this.mouseDownBrush ??= new SolidBrush(this.selectedColor);
                return this.mouseDownBrush;
            }
        }

        public SolidBrush OutLineBrush
        {
            get
            {
                this.outlineBrush ??= new SolidBrush(this.outlineColor);
                return this.outlineBrush;
            }
        }

        public SolidBrush InnerBrush
        {
            get
            {
                this.innerBrush ??= new SolidBrush(this.innerColor);
                return this.innerBrush;
            }
        }

        public StringFormat TextFormat
        {
            get
            {
                if (this.textFormat == null)
                {
                    this.textFormat = new()
                    {
                        Trimming = this.textTrimming,
                        Alignment = this.textAlignment,
                        LineAlignment = this.textLineAlignment,
                        FormatFlags = this.textFormatFlags
                    };
                }
                else if (!this.textFormat.Trimming.Equals(this.textTrimming) ||
                         !this.textFormat.Alignment.Equals(this.textAlignment) ||
                         !this.textFormat.LineAlignment.Equals(this.textLineAlignment) ||
                         !this.textFormat.FormatFlags.Equals(this.textFormatFlags))
                {
                    this.textFormat.Dispose();
                    this.textFormat = null;
                    this.textFormat = new StringFormat
                    {
                        Trimming = this.textTrimming,
                        Alignment = this.textAlignment,
                        LineAlignment = this.textLineAlignment,
                        FormatFlags = this.textFormatFlags
                    };
                }

                return this.textFormat;
            }
        }
    }
}
