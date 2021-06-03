using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.SearchTool
{
    public class SelectedFileHistoryEventArgs : SelectedItemEventArgs<DateTime>
    {
        public SelectedFileHistoryEventArgs(ContentsOpenType openType, DateTime viewDate) 
            : base(openType, viewDate) { }
    }
}
