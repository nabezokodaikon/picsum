using System;
using System.Collections.Generic;
using System.Linq;

namespace SWF.UIComponent.FlowList
{
    /// <summary>
    /// 選択項目インデックスリスト
    /// </summary>
    internal class ItemIndexList
    {
        public event EventHandler Change;

        private readonly List<int> _list = new List<int>();
        private List<int> _beforeList = null;

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public IList<int> GetList()
        {
            List<int> list = new List<int>(_list.ToArray());
            list.Sort();
            return list;
        }

        public bool Contains(int itemIndex)
        {
            return _list.Contains(itemIndex);
        }

        public void Add(int itemIndex)
        {
            if (_list.Contains(itemIndex))
            {
                throw new ArgumentException("既に存在する項目を追加しようとしました。");
            }

            _list.Add(itemIndex);
        }

        public void AddRange(int itemIndex)
        {
            if (_list.Count > 0)
            {
                int min = _list.Min();
                int max = _list.Max();
                if (itemIndex < min)
                {
                    for (int index = itemIndex; index <= max; index++)
                    {
                        if (!_list.Contains(index))
                        {
                            _list.Add(index);
                        }
                    }
                }
                else if (itemIndex > max)
                {
                    for (int index = min; index <= itemIndex; index++)
                    {
                        if (!_list.Contains(index))
                        {
                            _list.Add(index);
                        }
                    }
                }
            }
            else
            {
                _list.Add(itemIndex);
            }
        }

        public void Remove(int itemIndex)
        {
            if (!_list.Contains(itemIndex))
            {
                throw new ArgumentException("存在しない項目を削除しようとしました。");
            }

            _list.Remove(itemIndex);
        }

        public void Union(ItemIndexList other)
        {
            bool isChange = false;

            foreach (int itemIndex in other.GetList())
            {
                if (!_list.Contains(itemIndex))
                {
                    _list.Add(itemIndex);
                    isChange = true;
                }
            }

            if (isChange)
            {
                OnChange(new EventArgs());
            }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void BeginUpdate()
        {
            if (_beforeList != null)
            {
                throw new Exception("既に更新中です。");
            }

            _beforeList = new List<int>(_list.ToArray());
        }

        public void EndUpdate()
        {
            if (_beforeList == null)
            {
                throw new Exception("更新中ではありません。");
            }

            if (_beforeList.Count != _list.Count)
            {
                OnChange(new EventArgs());
            }
            else
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_beforeList[i] != _list[i])
                    {
                        OnChange(new EventArgs());
                        break;
                    }
                }
            }

            _beforeList = null;
        }

        protected virtual void OnChange(EventArgs e)
        {
            if (Change != null)
            {
                Change(this, e);
            }
        }
    }
}
