using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブ情報クラス
    /// </summary>
    public class TabInfo
    {
        #region インスタンス変数

        private readonly TabDrawArea _drawArea = new TabDrawArea();
        private readonly IList<IContentsParameter> _contentsParameterList = new List<IContentsParameter>();
        private int _contentsParameterListIndex = 0;
        private ContentsPanel _contents = null;
        private TabSwitch _owner = null;

        #endregion

        #region プロパティ

        public string Title
        {
            get
            {
                if (_contents == null)
                {
                    throw new NullReferenceException("コンテンツが設定されていません。");
                }

                return _contents.Title;
            }
        }

        public Image Icon
        {
            get
            {
                if (_contents == null)
                {
                    throw new NullReferenceException("コンテンツが設定されていません。");
                }

                return _contents.Icon;
            }
        }

        public TabSwitch Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        public bool HasNextContents
        {
            get
            {
                return _contentsParameterListIndex < _contentsParameterList.Count - 1;
            }
        }

        public bool HasPreviewContents
        {
            get
            {
                return _contentsParameterList.Count > 1 && _contentsParameterListIndex > 0;
            }
        }

        public bool HasContents
        {
            get
            {
                return this._contents != null;
            }
        }

        public ContentsPanel Contents
        {
            get
            {
                if (_contents == null)
                {
                    throw new NullReferenceException("コンテンツが設定されていません。");
                }

                return _contents;
            }
        }

        internal TabDrawArea DrawArea
        {
            get
            {
                return _drawArea;
            }
        }

        #endregion

        #region コンストラクタ

        internal TabInfo(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            _contentsParameterList.Add(param);
            _contentsParameterListIndex = 0;
            _contents = param.CreateContents();
        }

        #endregion

        #region メソッド

        public T GetContents<T>() where T : ContentsPanel
        {
            if (_contents == null)
            {
                throw new NullReferenceException("コンテンツが設定されていません。");
            }

            return (T)_contents;
        }

        public void OverwriteContents(IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            if (_contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            if (this._contentsParameterList.Last().Key != param.Key)
            {
                _contentsParameterList.Add(param);
                _contentsParameterListIndex = _contentsParameterList.Count - 1;
            }

            _contents = param.CreateContents();
        }

        public void Close()
        {
            clearContents();
        }

        public void DrawingTabContents(DrawTabEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (_contents == null)
            {
                throw new NullReferenceException("コンテンツが設定されていません。");
            }

            _contents.DrawingTabContents(e);
        }

        internal Bitmap GetContentsCapture()
        {
            if (_contents == null)
            {
                throw new NullReferenceException("コンテンツが設定されていません。");
            }

            int w = _contents.Width;
            int h = _contents.Height;
            Bitmap bmp = new Bitmap(w, h);
            _contents.DrawToBitmap(bmp, _contents.ClientRectangle);
            return bmp;
        }

        internal void ClearContents()
        {
            clearContents();
        }

        internal void CreatePreviewContents()
        {
            if (_contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            if (_contentsParameterList.Count < 1 ||
                _contentsParameterListIndex - 1 < 0)
            {
                throw new Exception("コンテンツパラメータの前の履歴が存在しません。");
            }

            _contentsParameterListIndex--;
            _contents = _contentsParameterList[_contentsParameterListIndex].CreateContents();
        }

        internal void CreateNextContents()
        {
            if (_contents != null)
            {
                throw new Exception("既にコンテンツが存在しています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            if (_contentsParameterList.Count < 1 ||
                _contentsParameterListIndex + 1 > _contentsParameterList.Count - 1)
            {
                throw new Exception("コンテンツパラメータの次の履歴が存在しません。");
            }

            _contentsParameterListIndex++;
            _contents = _contentsParameterList[_contentsParameterListIndex].CreateContents();
        }

        internal void CloneCurrentContents()
        {
            if (_contents != null)
            {
                throw new Exception("現在のコンテンツが残っています。ClearContentsメソッドでコンテンツをクリアして下さい。");
            }

            if (_contentsParameterList.Count < 1)
            {
                throw new Exception("コンテンツパラメータが存在しません。");
            }

            _contents = _contentsParameterList[_contentsParameterListIndex].CreateContents();
        }

        private void clearContents()
        {
            if (_contents != null)
            {
                _contents.Dispose();
                _contents = null;
            }
        }

        #endregion
    }
}
