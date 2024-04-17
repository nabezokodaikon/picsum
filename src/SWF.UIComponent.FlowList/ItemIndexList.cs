using System;
using System.Collections.Generic;
using System.Linq;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 選択項目インデックスリスト
    /// </summary>
    internal sealed class ItemIndexList
    {
        public event EventHandler Change;

        private readonly List<int> list = [];
        private List<int> beforeList = null;

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public IList<int> GetList()
        {
            var list = new List<int>([.. this.list]);
            list.Sort();
            return list;
        }

        public bool Contains(int itemIndex)
        {
            return this.list.Contains(itemIndex);
        }

        public void Add(int itemIndex)
        {
            if (this.list.Contains(itemIndex))
            {
                throw new ArgumentException("既に存在する項目を追加しようとしました。");
            }

            this.list.Add(itemIndex);
        }

        public void AddRange(int itemIndex)
        {
            if (this.list.Count > 0)
            {
                var min = this.list.Min();
                var max = this.list.Max();
                if (itemIndex < min)
                {
                    for (var index = itemIndex; index <= max; index++)
                    {
                        if (!this.list.Contains(index))
                        {
                            this.list.Add(index);
                        }
                    }
                }
                else if (itemIndex > max)
                {
                    for (var index = min; index <= itemIndex; index++)
                    {
                        if (!this.list.Contains(index))
                        {
                            this.list.Add(index);
                        }
                    }
                }
            }
            else
            {
                this.list.Add(itemIndex);
            }
        }

        public void Remove(int itemIndex)
        {
            if (!this.list.Contains(itemIndex))
            {
                throw new ArgumentException("存在しない項目を削除しようとしました。");
            }

            this.list.Remove(itemIndex);
        }

        public void Union(ItemIndexList other)
        {
            var isChange = false;

            foreach (var itemIndex in other.GetList())
            {
                if (!this.list.Contains(itemIndex))
                {
                    this.list.Add(itemIndex);
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
            this.list.Clear();
        }

        public void BeginUpdate()
        {
            if (this.beforeList != null)
            {
                throw new InvalidOperationException("既に更新中です。");
            }

            this.beforeList = new List<int>([.. this.list]);
        }

        public void EndUpdate()
        {
            if (this.beforeList == null)
            {
                throw new InvalidOperationException("更新中ではありません。");
            }

            if (this.beforeList.Count != this.list.Count)
            {
                this.OnChange(EventArgs.Empty);
            }
            else
            {
                for (var i = 0; i < this.list.Count; i++)
                {
                    if (this.beforeList[i] != this.list[i])
                    {
                        this.OnChange(EventArgs.Empty);
                        break;
                    }
                }
            }

            this.beforeList = null;
        }

        private void OnChange(EventArgs e)
        {
            this.Change?.Invoke(this, e);
        }
    }
}
