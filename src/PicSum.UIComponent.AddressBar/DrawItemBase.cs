using SWF.UIComponent.Core;
using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal abstract class DrawItemBase
        : IDisposable
    {
        protected const int MAXIMUM_SHOW_ITEM_COUNT = 16;

        public event EventHandler DropDownOpened;
        public event EventHandler DropDownClosed;
        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        private AddressBar _addressBar = null;
        private bool _isMousePoint = false;
        private bool _isMouseDown = false;
        private int _x = 0;
        private int _y = 0;
        private int _width = 0;
        private int _height = 0;

        public AddressBar AddressBar
        {
            get
            {
                return this._addressBar;
            }
            set
            {
                this._addressBar = value;
            }
        }

        public bool IsMousePoint
        {
            get
            {
                return this._isMousePoint;
            }
            set
            {
                this._isMousePoint = value;
            }
        }

        public bool IsMouseDown
        {
            get
            {
                return this._isMouseDown;
            }
            set
            {
                this._isMouseDown = value;
            }
        }

        public int X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        public int Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }

        public int Left
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }

        public int Top
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }

        public int Right
        {
            get
            {
                return this._x + this._width;
            }
            set
            {
                this._x = value - this._width;
            }
        }

        public int Bottom
        {
            get
            {
                return this._y + this._height;
            }
            set
            {
                this._y = value - this._height;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }

        public int Height
        {
            get
            {
                return this._height;
            }
            set
            {
                this._height = value;
            }
        }

        public abstract void Draw(Graphics g);

        public abstract void OnMouseDown(MouseEventArgs e);

        public abstract void OnMouseClick(MouseEventArgs e);

        public RectangleF GetRectangle()
        {
            return new RectangleF(this._x, this._y, this._width, this._height);
        }

        public void ClearRectangle()
        {
            this._x = 0;
            this._y = 0;
            this._width = 0;
            this._height = 0;
        }

        public void Dispose()
        {
            this.DropDownOpened = null;
            this.DropDownClosed = null;
            this.SelectedDirectory = null;
            this._addressBar = null;

            GC.SuppressFinalize(this);
        }

        protected int GetDropDownItemHeight()
        {
            const int DROPDOWN_ITEM_HEIGHT = 32;
            var scale = WindowUtil.GetCurrentWindowScale(this._addressBar);
            return (int)(DROPDOWN_ITEM_HEIGHT * scale);
        }

        protected int GetMinimumDropDownWidth()
        {
            const int MINIMUM_DROPDOWN_WIDHT = 128;
            var scale = WindowUtil.GetCurrentWindowScale(this._addressBar);
            return (int)(MINIMUM_DROPDOWN_WIDHT * scale);
        }

        protected virtual void OnDropDownOpened(EventArgs e)
        {
            this.DropDownOpened?.Invoke(this, e);
        }

        protected virtual void OnDropDownClosed(EventArgs e)
        {
            this.DropDownClosed?.Invoke(this, e);
        }

        protected virtual void OnSelectedDirectory(SelectedDirectoryEventArgs e)
        {
            this.SelectedDirectory?.Invoke(this, e);
        }
    }
}
