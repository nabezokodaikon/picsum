using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    abstract class DrawItemBase
    {
        protected const int DROPDOWN_ITEM_HEIGHT = 32;
        protected const int MINIMUM_DROPDOWN_WIDHT = 128;
        protected const int MAXIMUM_SHOW_ITEM_COUNT = 16;

        public event EventHandler DropDownOpened;
        public event EventHandler DropDownClosed;
        public event EventHandler<SelectedDirectoryEventArgs> SelectedDirectory;

        private AddressBar addressBar = null;
        private bool isMousePoint = false;
        private bool isMouseDown = false;
        private int x = 0;
        private int y = 0;
        private int width = 0;
        private int height = 0;

        public AddressBar AddressBar
        {
            get
            {
                return this.addressBar;
            }
            set
            {
                this.addressBar = value;
            }
        }

        public bool IsMousePoint
        {
            get
            {
                return this.isMousePoint;
            }
            set
            {
                this.isMousePoint = value;
            }
        }

        public bool IsMouseDown
        {
            get
            {
                return this.isMouseDown;
            }
            set
            {
                this.isMouseDown = value;
            }
        }

        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public int Left
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public int Top
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public int Right
        {
            get
            {
                return this.x + this.width;
            }
            set
            {
                this.x = value - this.width;
            }
        }

        public int Bottom
        {
            get
            {
                return this.y + this.height;
            }
            set
            {
                this.y = value - this.height;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public abstract void Draw(Graphics g);

        public abstract void OnMouseDown(MouseEventArgs e);

        public abstract void OnMouseClick(MouseEventArgs e);

        public RectangleF GetRectangle()
        {
            return new RectangleF(this.x, this.y, this.width, this.height);
        }

        public void ClearRectangle()
        {
            this.x = 0;
            this.y = 0;
            this.width = 0;
            this.height = 0;
        }

        public void Dispose()
        {
            this.DropDownOpened = null;
            this.DropDownClosed = null;
            this.SelectedDirectory = null;
            this.addressBar = null;
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
