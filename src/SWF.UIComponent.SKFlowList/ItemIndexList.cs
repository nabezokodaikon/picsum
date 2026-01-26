using System;
using System.Collections.Generic;
using System.Linq;

namespace SWF.UIComponent.SKFlowList
{
    /// <summary>
    /// 選択項目インデックスリスト
    /// </summary>
    internal sealed class ItemIndexList
    {
        public event EventHandler Change;

        private readonly List<int> _list = [];
        private int[] _beforeList = null;

        public int Count
        {
            get
            {
                return this._list.Count;
            }
        }

        public int[] GetList()
        {
            var list = new List<int>([.. this._list]);
            list.Sort();
            return [.. list];
        }

        public bool Contains(int itemIndex)
        {
            return this._list.Contains(itemIndex);
        }

        public void Add(int itemIndex)
        {
            if (this._list.Contains(itemIndex))
            {
                throw new ArgumentException("既に存在する項目を追加しようとしました。");
            }

            this._list.Add(itemIndex);
        }

        public void AddRange(int itemIndex)
        {
            if (this._list.Count > 0)
            {
                var min = this._list.Min();
                var max = this._list.Max();
                if (itemIndex < min)
                {
                    for (var index = itemIndex; index <= max; index++)
                    {
                        if (!this._list.Contains(index))
                        {
                            this._list.Add(index);
                        }
                    }
                }
                else if (itemIndex > max)
                {
                    for (var index = min; index <= itemIndex; index++)
                    {
                        if (!this._list.Contains(index))
                        {
                            this._list.Add(index);
                        }
                    }
                }
            }
            else
            {
                this._list.Add(itemIndex);
            }
        }

        public void Remove(int itemIndex)
        {
            if (!this._list.Contains(itemIndex))
            {
                throw new ArgumentException("存在しない項目を削除しようとしました。");
            }

            this._list.Remove(itemIndex);
        }

        public void Union(ItemIndexList other)
        {
            var isChange = false;

            foreach (var itemIndex in other.GetList())
            {
                if (!this._list.Contains(itemIndex))
                {
                    this._list.Add(itemIndex);
                    isChange = true;
                }
            }

            if (isChange)
            {
                this.OnChange(EventArgs.Empty);
            }
        }

        public void Clear()
        {
            this._list.Clear();
        }

        public void BeginUpdate()
        {
            if (this._beforeList != null)
            {
                throw new InvalidOperationException("既に更新中です。");
            }

            this._beforeList = [.. this._list];
        }

        public void EndUpdate()
        {
            if (this._beforeList == null)
            {
                throw new InvalidOperationException("更新中ではありません。");
            }

            if (this._beforeList.Length != this._list.Count)
            {
                this.OnChange(EventArgs.Empty);
            }
            else
            {
                for (var i = 0; i < this._list.Count; i++)
                {
                    if (this._beforeList[i] != this._list[i])
                    {
                        this.OnChange(EventArgs.Empty);
                        break;
                    }
                }
            }

            this._beforeList = null;
        }

        private void OnChange(EventArgs e)
        {
            this.Change?.Invoke(this, e);
        }
    }
}
