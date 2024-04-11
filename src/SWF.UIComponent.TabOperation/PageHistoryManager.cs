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
        private readonly List<IPageParameter> list = new List<IPageParameter>();
        private int index = -1;

        /// <summary>
        /// 前のコンテンツへ移動できるか確認します。
        /// </summary>
        public bool CanPreview
        {
            get
            {
                return this.list.Count > 1 && this.index > 0;
            }
        }

        /// <summary>
        /// 次のコンテンツへ移動できるか確認します。
        /// </summary>
        public bool CanNext
        {
            get
            {
                return this.index < this.list.Count - 1;
            }
        }

        internal PageHistoryManager()
        {

        }

        public void Add(IPageParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (this.list.Count == 0)
            {
                // 履歴が存在しない場合。
                this.list.Add(param);
                this.index = 0;
            }
            else if (this.list.Count == 1)
            {
                // 履歴が1件の場合。
                if (this.list.First().Key != param.Key)
                {
                    this.list.Add(param);
                    this.index = 1;
                }
            }
            else
            {
                // 履歴が2件以上の場合。
                if (this.index == 0)
                {
                    // 最初の履歴の場合。
                    var first = this.list.First();
                    this.list.Clear();
                    this.list.AddRange(new IPageParameter[] { first, param });
                    this.index = 1;
                }
                else if (this.list.Count - 1 == this.index)
                {
                    // 現在の履歴が最後の場合。
                    if (this.list.Last().Key != param.Key)
                    {
                        this.list.Add(param);
                        this.index++;
                    }
                }
                else
                {
                    // 履歴が途中の場合。
                    if (this.list[this.index].Key != param.Key)
                    {
                        this.list.RemoveRange(this.index + 1, this.list.Count - this.index - 1);
                        this.list.Add(param);
                        this.index = this.list.Count - 1;
                    }
                }
            }
        }

        public PagePanel CreatePreview()
        {
            if (this.list.Count < 1 ||
                this.index - 1 < 0)
            {
                throw new IndexOutOfRangeException("コンテンツパラメータの前の履歴が存在しません。");
            }

            this.index--;
            return this.list[this.index].CreatePage();
        }

        public PagePanel CreateNext()
        {
            if (this.list.Count < 1 ||
                this.index + 1 > this.list.Count - 1)
            {
                throw new IndexOutOfRangeException("コンテンツパラメータの次の履歴が存在しません。");
            }

            this.index++;
            return this.list[this.index].CreatePage();
        }

        public PagePanel CreateClone()
        {
            if (this.list.Count < 1 ||
                this.index > this.list.Count - 1)
            {
                throw new IndexOutOfRangeException("コンテンツパラメータの現在の履歴が存在しません。");
            }

            return this.list[this.index].CreatePage();
        }
    }
}
