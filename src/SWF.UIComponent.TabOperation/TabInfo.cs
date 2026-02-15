using System;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ情報クラス
    /// </summary>

    public sealed class TabInfo
    {

        private readonly TabDrawArea _drawArea;
        private readonly PageHistoryManager _historyManager = new();
        private PagePanel _page = null;
        private TabSwitch _owner = null;

        public string Title
        {
            get
            {
                if (this._page == null)
                {
                    throw new InvalidOperationException("ページが設定されていません。");
                }

                return this._page.Title;
            }
        }

        public Image Icon
        {
            get
            {
                if (this._page == null)
                {
                    return null;
                }

                return this._page.Icon;
            }
        }

        public TabSwitch Owner
        {
            get
            {
                return this._owner;
            }
            set
            {
                this._owner = value;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return this._historyManager.CanNext;
            }
        }

        public bool HasPreviewPage
        {
            get
            {
                return this._historyManager.CanPreview;
            }
        }

        public bool HasPage
        {
            get
            {
                return this._page != null;
            }
        }

        public TabDrawArea DrawArea
        {
            get
            {
                return this._drawArea;
            }
        }

        internal TabInfo(TabSwitch tabSwitch, IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this._drawArea = new(tabSwitch);
            this._historyManager.Add(param);
            this._page = param.CreatePage();
        }

        public PagePanel GetPage()
        {
            if (this._page == null)
            {
                throw new InvalidOperationException("ページが設定されていません。");
            }

            return this._page;
        }

        public T GetPage<T>() where T : PagePanel
        {
            if (this._page == null)
            {
                throw new InvalidOperationException("ページが設定されていません。");
            }

            return (T)this._page;
        }

        public void OverwritePage(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (this._page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this._historyManager.Add(param);
            this._page = param.CreatePage();
        }

        public void Close()
        {
            this.ClearPage();
        }

        public void DrawingTabPage(DrawTabEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (this._page == null)
            {
                throw new InvalidOperationException("ページが設定されていません。");
            }

            this._page.DrawingTabPage(e);
        }

        internal Bitmap GetPageCapture()
        {
            if (this._page == null)
            {
                throw new InvalidOperationException("ページが設定されていません。");
            }

            var w = this._page.Width;
            var h = this._page.Height;
            var bmp = new Bitmap(w, h);
            this._page.DrawToBitmap(
                bmp,
                new Rectangle(this._page.Location.X, this._page.Location.Y, w, h));
            return bmp;
        }

        internal void ClearPage()
        {
            if (this._page != null)
            {
                this._page.Dispose();
                this._page = null;
            }
        }

        internal void CreatePreviewPage()
        {
            if (this._page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this._page = this._historyManager.CreatePreview();
        }

        internal void CreateNextPage()
        {
            if (this._page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this._page = this._historyManager.CreateNext();
        }

        internal void CloneCurrentPage()
        {
            if (this._page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this._page = this._historyManager.CreateClone();
        }

    }
}
