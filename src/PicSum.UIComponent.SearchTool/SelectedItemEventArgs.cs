using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.SearchTool
{
    public class SelectedItemEventArgs<TValue> : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private TValue _value;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public TValue Value
        {
            get
            {
                return _value;
            }
        }

        public SelectedItemEventArgs(ContentsOpenType openType, TValue value)
        {
            _openType = openType;
            _value = value;
        }
    }
}
