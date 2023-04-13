using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace PicSum.UIComponent.AddressBar
{
    abstract class DrawItemBase
    {
        protected const int DROPDOWN_ITEM_HEIGHT = 24;
        protected const int MINIMUM_DROPDOWN_WIDHT = 128;
        protected const int MAXIMUM_SHOW_ITEM_COUNT = 16;

        public event EventHandler DropDownOpened;
        public event EventHandler DropDownClosed;
        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        private IContainer _components = null;
        private AddressBar _addressBar = null;
        private Palette _palette = null;
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
                return _addressBar;
            }
            set
            {
                _addressBar = value;
            }
        }

        public Palette Palette
        {
            get
            {
                return _palette;
            }
            set
            {
                _palette = value;
            }
        }

        public bool IsMousePoint
        {
            get
            {
                return _isMousePoint;
            }
            set
            {
                _isMousePoint = value;
            }
        }

        public bool IsMouseDown
        {
            get
            {
                return _isMouseDown;
            }
            set
            {
                _isMouseDown = value;
            }
        }

        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public int Left
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public int Top
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public int Right
        {
            get
            {
                return _x + _width;
            }
            set
            {
                _x = value - _width;
            }
        }

        public int Bottom
        {
            get
            {
                return _y + _height;
            }
            set
            {
                _y = value - _height;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        protected IContainer components
        {
            get
            {
                if (_components == null)
                {
                    _components = new Container();
                }

                return _components;
            }
        }

        public abstract void Draw(Graphics g);

        public abstract void OnMouseDown(MouseEventArgs e);

        public abstract void OnMouseClick(MouseEventArgs e);

        public Rectangle GetRectangle()
        {
            return new Rectangle(_x, _y, _width, _height);
        }

        public void ClearRectangle()
        {
            _x = 0;
            _y = 0;
            _width = 0;
            _height = 0;
        }

        protected virtual void Dispose()
        {
            if (_components != null)
            {
                _components.Dispose();
            }
        }

        protected virtual void OnDropDownOpened(EventArgs e)
        {
            if (DropDownOpened != null)
            {
                DropDownOpened(this, e);
            }
        }

        protected virtual void OnDropDownClosed(EventArgs e)
        {
            if (DropDownClosed != null)
            {
                DropDownClosed(this, e);
            }
        }

        protected virtual void OnSelectedDirectory(SelectedDirectoryEventArgs e)
        {
            if (SelectedDirectory != null)
            {
                SelectedDirectory(this, e);
            }
        }
    }
}
