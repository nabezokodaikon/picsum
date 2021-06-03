using System.Drawing;

namespace PicSum.UIComponent.AddressBar
{
    public class Palette
    {
        private Font _textFont = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
        private Color _textColor = Color.FromArgb(64, 64, 64);
        private Color _mousePointColor = Color.FromArgb(48, 48, 96, 144);
        private Color _selectedColor = Color.FromArgb(192, 48, 96, 144);
        private Color _outlineColor = Color.FromArgb(64, 64, 64);
        private Color _innerColor = Color.White;
        private StringTrimming _textTrimming = StringTrimming.EllipsisCharacter;
        private StringAlignment _textAlignment = StringAlignment.Center;
        private StringAlignment _textLineAlignment = StringAlignment.Center;
        private StringFormatFlags _textFormatFlags = 0;
        private SolidBrush _textBrush = null;
        private SolidBrush _mousePointBrush = null;
        private SolidBrush _mouseDownBrush = null;
        private SolidBrush _outlineBrush = null;
        private SolidBrush _innerBrush = null;
        private StringFormat _textFormat = null;

        public Font TextFont
        {
            get
            {
                return _textFont;
            }
            set
            {
                _textFont = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
            }
        }

        public Color MousePointColor
        {
            get
            {
                return _mousePointColor;
            }
            set
            {
                _mousePointColor = value;
            }
        }

        public Color MouseDownColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
            }
        }

        public Color OutlineColor
        {
            get
            {
                return _outlineColor;
            }
            set
            {
                _outlineColor = value;
            }
        }

        public Color InnerColor
        {
            get
            {
                return _innerColor;
            }
            set
            {
                _innerColor = value;
            }
        }

        public StringTrimming TextTrimming
        {
            get
            {
                return _textTrimming;
            }
            set
            {
                _textTrimming = value;
            }
        }

        public StringAlignment TextAlignment
        {
            get
            {
                return _textAlignment;
            }
            set
            {
                _textAlignment = value;
            }
        }

        public StringAlignment TextLineAlignment
        {
            get
            {
                return _textLineAlignment;
            }
            set
            {
                _textLineAlignment = value;
            }
        }

        public StringFormatFlags TextFormatFlags
        {
            get
            {
                return _textFormatFlags;
            }
            set
            {
                _textFormatFlags = value;
            }
        }

        public SolidBrush TextBrush
        {
            get
            {
                if (_textBrush == null)
                {
                    _textBrush = new SolidBrush(_textColor);
                }
                else if (!_textBrush.Color.Equals(_textColor))
                {
                    _textBrush.Dispose();
                    _textBrush = null;
                    _textBrush = new SolidBrush(_textColor);
                }

                return _textBrush;
            }
        }

        public SolidBrush MousePointBrush
        {
            get
            {
                if (_mousePointBrush == null)
                {
                    _mousePointBrush = new SolidBrush(_mousePointColor);
                }
                else if (!_mousePointBrush.Color.Equals(_mousePointColor))
                {
                    _mousePointBrush.Dispose();
                    _mousePointBrush = null;
                    _mousePointBrush = new SolidBrush(_mousePointColor);
                }

                return _mousePointBrush;
            }
        }

        public SolidBrush MouseDownBrush
        {
            get
            {
                if (_mouseDownBrush == null)
                {
                    _mouseDownBrush = new SolidBrush(_selectedColor);
                }
                else if (!_mouseDownBrush.Color.Equals(_selectedColor))
                {
                    _mouseDownBrush.Dispose();
                    _mouseDownBrush = null;
                    _mouseDownBrush = new SolidBrush(_selectedColor);
                }

                return _mouseDownBrush;
            }
        }

        public SolidBrush OutLineBrush
        {
            get
            {
                if (_outlineBrush == null)
                {
                    _outlineBrush = new SolidBrush(_outlineColor);
                }
                else if (!_outlineBrush.Color.Equals(_outlineColor))
                {
                    _outlineBrush.Dispose();
                    _outlineBrush = null;
                    _outlineBrush = new SolidBrush(_outlineColor);
                }

                return _outlineBrush;
            }
        }

        public SolidBrush InnerBrush
        {
            get
            {
                if (_innerBrush == null)
                {
                    _innerBrush = new SolidBrush(_innerColor);
                }
                else if (!_innerBrush.Color.Equals(_innerColor))
                {
                    _innerBrush.Dispose();
                    _innerBrush = null;
                    _innerBrush = new SolidBrush(_innerColor);
                }

                return _innerBrush;
            }
        }

        public StringFormat TextFormat
        {
            get
            {
                if (_textFormat == null)
                {
                    _textFormat = new StringFormat();
                    _textFormat.Trimming = _textTrimming;
                    _textFormat.Alignment = _textAlignment;
                    _textFormat.LineAlignment = _textLineAlignment;
                    _textFormat.FormatFlags = _textFormatFlags;
                }
                else if (!_textFormat.Trimming.Equals(_textTrimming) ||
                         !_textFormat.Alignment.Equals(_textAlignment) ||
                         !_textFormat.LineAlignment.Equals(_textLineAlignment) ||
                         !_textFormat.FormatFlags.Equals(_textFormatFlags))
                {
                    _textFormat.Dispose();
                    _textFormat = null;
                    _textFormat = new StringFormat();
                    _textFormat.Trimming = _textTrimming;
                    _textFormat.Alignment = _textAlignment;
                    _textFormat.LineAlignment = _textLineAlignment;
                    _textFormat.FormatFlags = _textFormatFlags;
                }

                return _textFormat;
            }
        }
    }
}
