using System;
using System.Collections.Generic;
using System.Drawing;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Common;

namespace PicSum.UIComponent.SearchTool
{
    // TODO: WideDropDownでは、このコードをMainPanelで実装する。
    public class SearchTagToolButton : SearchToolButtonBase<string>
    {
        public event EventHandler<SelectedTagEventArgs> SelectedTag;

        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> _getTagListProcess = null;

        private TwoWayProcess<GetTagListAsyncFacade, ListEntity<string>> getTagListProcess
        {
            get
            {
                if (_getTagListProcess == null)
                {
                    _getTagListProcess = TaskManager.CreateTwoWayProcess<GetTagListAsyncFacade, ListEntity<string>>(ProcessContainer);
                    _getTagListProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<string>>(getTagListProcess_Callback);
                }

                return _getTagListProcess;
            }
        }

        public SearchTagToolButton()
            : base()
        {
            initializeComponent();
        }

        protected override void OnDropDownOpening()
        {
            getTagListProcess.Execute(this);
        }

        protected override void OnDrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            SearchInfoEntity<string> item = ItemList[e.ItemIndex];

            e.Graphics.DrawString(item.Value,
                                  this.Font,
                                  DropDownList.ItemTextBrush,
                                  e.ItemRectangle,
                                  DropDownList.ItemTextFormat);
        }

        protected override void OnSelectedItem(SelectedItemEventArgs<string> e)
        {
            OnSelectedTag(new SelectedTagEventArgs(e.OpenType, e.Value));
        }

        protected virtual void OnSelectedTag(SelectedTagEventArgs e)
        {
            if (SelectedTag != null)
            {
                SelectedTag(this, e);
            }
        }

        private void initializeComponent()
        {

        }

        private void getTagListProcess_Callback(object sender, ListEntity<string> e)
        {
            ItemList = new List<SearchInfoEntity<string>>();

            int width = MINIMUM_DROPDOWN_WIDTH;

            using (Graphics dc = this.CreateGraphics())
            {
                foreach (string value in e)
                {
                    width = Math.Max(width, (int)dc.MeasureString(value, this.Font).Width);
                    SearchInfoEntity<string> item = new SearchInfoEntity<string>();
                    item.Value = value;
                    ItemList.Add(item);
                }
            }

            if (e.Count > MAXIMUM_SHOW_DROPDOWN_ITEM_COUNT)
            {
                width += DropDownList.ScrollBarWidth;
            }

            int height = Math.Min(MAXIMUM_SHOW_DROPDOWN_ITEM_COUNT * DropDownList.ItemHeight,
                                  e.Count * DropDownList.ItemHeight);

            DropDownList.Size = new Size(width + DropDownList.ItemHeight + 4, height);
            DropDownList.ItemCount = e.Count;

            int selectedIndex = e.IndexOf(SelectedValue);
            if (selectedIndex > -1)
            {
                DropDownList.SelectItem(selectedIndex);
            }
        }
    }
}
