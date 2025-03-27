using System;
using System.Drawing;
using System.Runtime.Versioning;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ情報クラス
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class TabInfo
    {

        private readonly TabDrawArea drawArea;
        private readonly PageHistoryManager historyManager = new();
        private PagePanel page = null;
        private TabSwitch owner = null;

        public string Title
        {
            get
            {
                if (this.page == null)
                {
                    throw new NullReferenceException("ページが設定されていません。");
                }

                return this.page.Title;
            }
        }

        public Image Icon
        {
            get
            {
                if (this.page == null)
                {
                    throw new NullReferenceException("ページが設定されていません。");
                }

                return this.page.Icon;
            }
        }

        public TabSwitch Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return this.historyManager.CanNext;
            }
        }

        public bool HasPreviewPage
        {
            get
            {
                return this.historyManager.CanPreview;
            }
        }

        public bool HasPage
        {
            get
            {
                return this.page != null;
            }
        }

        public PagePanel Page
        {
            get
            {
                if (this.page == null)
                {
                    throw new NullReferenceException("ページが設定されていません。");
                }

                return this.page;
            }
        }

        internal TabDrawArea DrawArea
        {
            get
            {
                return this.drawArea;
            }
        }

        internal TabInfo(TabSwitch tabSwitch, IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(tabSwitch, nameof(tabSwitch));
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            this.drawArea = new(tabSwitch);
            this.historyManager.Add(param);
            this.page = param.CreatePage();
        }

        public T GetPage<T>() where T : PagePanel
        {
            if (this.page == null)
            {
                throw new NullReferenceException("ページが設定されていません。");
            }

            return (T)this.page;
        }

        public void OverwritePage(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (this.page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this.historyManager.Add(param);
            this.page = param.CreatePage();
        }

        public void Close()
        {
            this.ClearPage();
        }

        public void DrawingTabPage(DrawTabEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (this.page == null)
            {
                throw new NullReferenceException("ページが設定されていません。");
            }

            this.page.DrawingTabPage(e);
        }

        internal Bitmap GetPageCapture()
        {
            if (this.page == null)
            {
                throw new NullReferenceException("ページが設定されていません。");
            }

            var w = this.page.Width;
            var h = this.page.Height;
            var bmp = new Bitmap(w, h);
            this.page.DrawToBitmap(bmp, this.page.ClientRectangle);
            return bmp;
        }

        internal void ClearPage()
        {
            if (this.page != null)
            {
                this.page.Dispose();
                this.page = null;
            }
        }

        internal void CreatePreviewPage()
        {
            if (this.page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this.page = this.historyManager.CreatePreview();
        }

        internal void CreateNextPage()
        {
            if (this.page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this.page = this.historyManager.CreateNext();
        }

        internal void CloneCurrentPage()
        {
            if (this.page != null)
            {
                throw new InvalidOperationException("既にページが存在しています。ClearPageメソッドでページをクリアして下さい。");
            }

            this.page = this.historyManager.CreateClone();
        }

    }
}
