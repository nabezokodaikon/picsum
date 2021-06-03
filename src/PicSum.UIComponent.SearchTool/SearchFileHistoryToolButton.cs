using System;
using System.Collections.Generic;
using System.Drawing;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncFacade;
using PicSum.Task.Entity;
using PicSum.UIComponent.Common;

namespace PicSum.UIComponent.SearchTool
{
    public class SearchFileHistoryToolButton : SearchToolButtonBase<DateTime>
    {
        public event EventHandler<SelectedFileHistoryEventArgs> SelectedFileHistory;

        private TwoWayProcess<GetFileViewHistoryAsyncFacade, ListEntity<DateTime>> _getFileHistoryProcess = null;

        private TwoWayProcess<GetFileViewHistoryAsyncFacade, ListEntity<DateTime>> getFileHistoryProcess
        {
            get
            {
                if (_getFileHistoryProcess == null)
                {
                    _getFileHistoryProcess = TaskManager.CreateTwoWayProcess<GetFileViewHistoryAsyncFacade, ListEntity<DateTime>>(ProcessContainer);
                    _getFileHistoryProcess.Callback += new AsyncTaskCallbackEventHandler<ListEntity<DateTime>>(getFileHistoryProcess_Callback);
                }

                return _getFileHistoryProcess;
            }
        }

        public SearchFileHistoryToolButton()
            : base()
        {
            initializeComponent();
        }

        protected override void OnDropDownOpening()
        {
            getFileHistoryProcess.Execute(this);
        }

        protected override void OnDrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (e.IsFocus || e.IsMousePoint || e.IsSelected)
            {
                e.Graphics.FillRectangle(DropDownList.SelectedItemBrush, e.ItemRectangle);
            }

            SearchInfoEntity<DateTime> item = ItemList[e.ItemIndex];

            e.Graphics.DrawString(item.Value.ToString("yyyy/MM/dd"),
                                  this.Font,
                                  DropDownList.ItemTextBrush,
                                  e.ItemRectangle,
                                  DropDownList.ItemTextFormat);
        }

        protected override void OnSelectedItem(SelectedItemEventArgs<DateTime> e)
        {
            OnSelectedFileHistory(new SelectedFileHistoryEventArgs(e.OpenType, e.Value));
        }

        protected virtual void OnSelectedFileHistory(SelectedFileHistoryEventArgs e)
        {
            if (SelectedFileHistory != null)
            {
                SelectedFileHistory(this, e);
            }
        }

        private void initializeComponent()
        {

        }

        private void getFileHistoryProcess_Callback(object sender, ListEntity<DateTime> e)
        {
            ItemList = new List<SearchInfoEntity<DateTime>>();

            foreach (DateTime value in e)
            {
                SearchInfoEntity<DateTime> item = new SearchInfoEntity<DateTime>();
                item.Value = value;
                ItemList.Add(item);
            }

            int width = getDropDownWidht();
            if (e.Count > MAXIMUM_SHOW_DROPDOWN_ITEM_COUNT)
            {
                width += DropDownList.ScrollBarWidth;
            }

            int height = Math.Min(MAXIMUM_SHOW_DROPDOWN_ITEM_COUNT * DropDownList.ItemHeight,
                                  e.Count * DropDownList.ItemHeight);

            DropDownList.Size = new Size(width, height);
            DropDownList.ItemCount = ItemList.Count;

            int selectedIndex = e.IndexOf(SelectedValue);
            if (selectedIndex > -1)
            {
                DropDownList.SelectItem(selectedIndex);
            }
        }

        private int getDropDownWidht()
        {
            using (Graphics dc = this.CreateGraphics())
            {
                return (int)dc.MeasureString("WWWW/WW/WW", this.Font).Width;
            }
        }
    }
}
