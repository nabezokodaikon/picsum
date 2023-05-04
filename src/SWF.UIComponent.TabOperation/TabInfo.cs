using System;
using System.Drawing;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ情報クラス
    /// </summary>
    public sealed class TabInfo
    {
        #region インスタンス変数

        private readonly TabDrawArea drawArea = new TabDrawArea();
        private readonly ContentsHistoryManager historyManager = new ContentsHistoryManager();
        private ContentsPanel contents = null;
        private TabSwitch owner = null;

        #endregion

        #region プロパティ

        public string Title
        {
            get
            {
                if (this.contents == null)
                {
                    throw new NullReferenceException("コンテンツが設定されていません。");
                }

                return this.contents.Title;
            }
        }

        public Image Icon
        {
            get
            {
                if (this.contents == null)
                {
                    throw new NullReferenceException("コンテンツが設定されていません。");
                }

                return this.contents.Icon;
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

        public bool HasNextContents
        {
            get
            {
                return this.historyManager.CanNext;
            }
        }

        public bool HasPreviewContents
        {
            get
            {
                return this.historyManager.CanPreview;
            }
        }

        public bool HasContents
        {
            get
            {
                return this.contents != null;
            }
        }

        public ContentsPanel Contents
        {
            get
            {
                if (this.contents == null)
                {
                    throw new NullReferenceException("コンテンツが設定されていません。");
                }

                return this.contents;
            }
        }

        internal TabDrawArea DrawArea
        {
            get
            {
                return this.drawArea;
            }
        }

        #endregion

        #region コンストラクタ

        internal TabInfo(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            this.historyManager.Add(param);
            this.contents = param.CreateContents();
        }

        #endregion

        #region メソッド

        public T GetContents<T>() where T : ContentsPanel
        {
            if (this.contents == null)
            {
                throw new NullReferenceException("コンテンツが設定されていません。");
            }

            return (T)this.contents;
        }

        public void OverwriteContents(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (this.contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            this.historyManager.Add(param);
            this.contents = param.CreateContents();
        }

        public void Close()
        {
            this.ClearContents();
        }

        public void DrawingTabContents(DrawTabEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (this.contents == null)
            {
                throw new NullReferenceException("コンテンツが設定されていません。");
            }

            this.contents.DrawingTabContents(e);
        }

        internal Bitmap GetContentsCapture()
        {
            if (this.contents == null)
            {
                throw new NullReferenceException("コンテンツが設定されていません。");
            }

            var w = this.contents.Width;
            var h = this.contents.Height;
            var bmp = new Bitmap(w, h);
            this.contents.DrawToBitmap(bmp, this.contents.ClientRectangle);
            return bmp;
        }

        internal void ClearContents()
        {
            if (this.contents != null)
            {
                this.contents.Dispose();
                this.contents = null;
            }
        }

        internal void CreatePreviewContents()
        {
            if (this.contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            this.contents = this.historyManager.CreatePreview();
        }

        internal void CreateNextContents()
        {
            if (this.contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            this.contents = this.historyManager.CreateNext();
        }

        internal void CloneCurrentContents()
        {
            if (this.contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            this.contents = this.historyManager.CreateClone();
        }

        #endregion
    }
}
