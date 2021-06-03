using System;
using System.Collections.Generic;
using System.Drawing;
using PicSum.Core.Base.Conf;
using PicSum.UIComponent.Common;
using PicSum.UIComponent.SearchTool.Properties;
using SWF.Common;

namespace PicSum.UIComponent.SearchTool
{
    public class SearchRatingToolButton : SearchToolButtonBase<int>
    {
        public event EventHandler<SelectedRatingEventArgs> SelectedRating;

        private Image _ratingImage = Resources.ActiveRatingIcon;

        public SearchRatingToolButton()
            : base()
        {
            initializeComponent();
        }

        protected override void OnDropDownOpening()
        {
            // 処理無し
        }

        protected override void OnDrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            SearchInfoEntity<int> item = ItemList[e.ItemIndex];

            for (int i = 0; i < item.Value; i++)
            {
                float w = Math.Min(_ratingImage.Width, DropDownList.ItemHeight);
                float h = w;
                float x = e.ItemRectangle.X + DropDownList.ItemHeight * i + (DropDownList.ItemHeight - w) / 2f;
                float y = e.ItemRectangle.Y + (DropDownList.ItemHeight - h) / 2f;
                e.Graphics.DrawImage(_ratingImage, x, y, w, h);
            }
        }

        protected override void OnSelectedItem(SelectedItemEventArgs<int> e)
        {
            OnSelectedRating(new SelectedRatingEventArgs(e.OpenType, e.Value));
        }

        protected virtual void OnSelectedRating(SelectedRatingEventArgs e)
        {
            if (SelectedRating != null)
            {
                SelectedRating(this, e);
            }
        }

        private void initializeComponent()
        {
            ItemList = new List<SearchInfoEntity<int>>();
            foreach (int value in new Range(ApplicationConst.MaximumRatingValue, ApplicationConst.MinimumRatingValue))
            {
                SearchInfoEntity<int> item = new SearchInfoEntity<int>();
                item.Value = value;
                ItemList.Add(item);
            }

            DropDownList.Size = new Size(DropDownList.ItemHeight * ItemList.Count,
                                         DropDownList.ItemHeight * ItemList.Count);
            DropDownList.ItemCount = ItemList.Count;
        }
    }
}
