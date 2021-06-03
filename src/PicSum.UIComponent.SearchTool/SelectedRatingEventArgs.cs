using System;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.SearchTool
{
    public class SelectedRatingEventArgs : SelectedItemEventArgs<int>
    {
        public SelectedRatingEventArgs(ContentsOpenType openType, int ratingValue)
            : base(openType, ratingValue) { }
    }
}
