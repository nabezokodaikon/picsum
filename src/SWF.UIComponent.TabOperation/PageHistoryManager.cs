using System;
using System.Collections.Generic;
using System.Linq;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// タブページ内の履歴を管理するクラス。
    /// </summary>
    internal sealed class PageHistoryManager
    {
        private readonly List<IPageParameter> _list = [];
        private int _index = -1;

        /// <summary>
        /// 前のコンテンツへ移動できるか確認します。
        /// </summary>
        public bool CanPreview
        {
            get
            {
                return this._list.Count > 1 && this._index > 0;
            }
        }

        /// <summary>
        /// 次のコンテンツへ移動できるか確認します。
        /// </summary>
        public bool CanNext
        {
            get
            {
                return this._index < this._list.Count - 1;
            }
        }

        internal PageHistoryManager()
        {

        }

        public void Add(IPageParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (this._list.Count == 0)
            {
                // 履歴が存在しない場合。
                this._list.Add(param);
                this._index = 0;
            }
            else if (this._list.Count == 1)
            {
                // 履歴が1件の場合。
                if (this._list.First().Key != param.Key)
                {
                    this._list.Add(param);
                    this._index = 1;
                }
            }
            else
            {
                // 履歴が2件以上の場合。
                if (this._index == 0)
                {
                    // 最初の履歴の場合。
                    var first = this._list.First();
                    this._list.Clear();
                    this._list.AddRange([first, param]);
                    this._index = 1;
                }
                else if (this._list.Count - 1 == this._index)
                {
                    // 現在の履歴が最後の場合。
                    if (this._list.Last().Key != param.Key)
                    {
                        this._list.Add(param);
                        this._index++;
                    }
                }
                else
                {
                    // 履歴が途中の場合。
                    if (this._list[this._index].Key != param.Key)
                    {
                        this._list.RemoveRange(this._index + 1, this._list.Count - this._index - 1);
                        this._list.Add(param);
                        this._index = this._list.Count - 1;
                    }
                }
            }
        }

        public PagePanel CreatePreview()
        {
            if (this._list.Count < 1 ||
                this._index - 1 < 0)
            {
                throw new IndexOutOfRangeException("コンテンツパラメータの前の履歴が存在しません。");
            }

            this._index--;
            return this._list[this._index].CreatePage();
        }

        public PagePanel CreateNext()
        {
            if (this._list.Count < 1 ||
                this._index + 1 > this._list.Count - 1)
            {
                throw new IndexOutOfRangeException("コンテンツパラメータの次の履歴が存在しません。");
            }

            this._index++;
            return this._list[this._index].CreatePage();
        }

        public PagePanel CreateClone()
        {
            if (this._list.Count < 1 ||
                this._index > this._list.Count - 1)
            {
                throw new IndexOutOfRangeException("コンテンツパラメータの現在の履歴が存在しません。");
            }

            return this._list[this._index].CreatePage();
        }
    }
}
