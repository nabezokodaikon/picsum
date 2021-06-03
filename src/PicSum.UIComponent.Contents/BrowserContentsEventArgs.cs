using System;
using PicSum.Core.Base.Conf;
using SWF.UIComponent.TabOperation;

namespace PicSum.UIComponent.Contents
{
    public class BrowserContentsEventArgs : EventArgs
    {
        private ContentsOpenType _openType = ContentsOpenType.Default;
        private IContentsParameter _parameter = null;

        public ContentsOpenType OpenType
        {
            get
            {
                return _openType;
            }
        }

        public IContentsParameter Parameter
        {
            get
            {
                return _parameter;
            }
        }

        public BrowserContentsEventArgs(ContentsOpenType openType, IContentsParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            _openType = openType;
            _parameter = param;
        }
    }
}
