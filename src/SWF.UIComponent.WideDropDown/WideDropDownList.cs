using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows")]
    public sealed class WideDropDownList
        : ToolStripDropDown
    {

        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;

        private readonly List<string> itemList = [];

        /// <summary>
        /// 項目テキスト色
        /// </summary>
        [Category("項目描画")]
        public Color ItemTextColor
        {
            get
            {
                return this.FlowList.ItemTextColor;
            }
        }

        /// <summary>
        /// 項目選択色
        /// </summary>
        [Category("項目描画")]
        public Color SelectedItemColor
        {
            get
            {
                return this.FlowList.SelectedItemColor;
            }
        }

        /// <summary>
        /// 項目マウスポイント色
        /// </summary>
        [Category("項目描画")]
        public Color MousePointItemColor
        {
            get
            {
                return this.FlowList.MousePointItemColor;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringTrimming ItemTextTrimming
        {
            get
            {
                return this.FlowList.ItemTextTrimming;
            }
            set
            {
                this.FlowList.ItemTextTrimming = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringAlignment ItemTextAlignment
        {
            get
            {
                return this.FlowList.ItemTextAlignment;
            }
            set
            {
                this.FlowList.ItemTextAlignment = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringAlignment ItemTextLineAlignment
        {
            get
            {
                return this.FlowList.ItemTextLineAlignment;
            }
            set
            {
                this.FlowList.ItemTextLineAlignment = value;
            }
        }

        /// <summary>
        /// 項目テキストフォーマット
        /// </summary>
        [Category("項目描画")]
        public StringFormatFlags ItemTextFormatFlags
        {
            get
            {
                return this.FlowList.ItemTextFormatFlags;
            }
            set
            {
                this.FlowList.ItemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public StringFormat ItemTextFormat
        {
            get
            {
                return this.FlowList.ItemTextFormat;
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.ToolStripItem.BackColor = value;
                this.FlowList.BackColor = value;
            }
        }

        /// <summary>
        /// スクロールバーの幅
        /// </summary>
        public int ScrollBarWidth
        {
            get
            {
                return this.FlowList.ScrollBarWidth;
            }
        }

        public Image Icon { get; set; }

        internal bool IsClickAndClose { get; set; }

        private ToolStripControlHost ToolStripItem
        {
            get
            {
                return (ToolStripControlHost)this.Items[0];
            }
        }

        private FlowList.FlowList FlowList
        {
            get
            {
                return (FlowList.FlowList)((ToolStripControlHost)this.Items[0]).Control;
            }
        }

        private int ItemTextHeight
        {
            get
            {
                return this.FontHeight * 2;
            }
        }

        public WideDropDownList()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint,
                true);

            this.SetStyle(
                ControlStyles.Selectable,
                false);

            this.UpdateStyles();

            this.Items.Add(new ToolStripControlHost(new FlowList.FlowList()));
            this.Padding = new Padding(2, 1, 2, 0);

            this.ToolStripItem.AutoSize = false;
            this.ToolStripItem.BackColor = this.BackColor;
            this.ToolStripItem.Size = new Size(800, 250);

            this.FlowList.BackColor = Color.White;
            this.FlowList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            this.FlowList.ItemTextLineAlignment = StringAlignment.Center;
            this.FlowList.ItemTextAlignment = StringAlignment.Near;
            this.FlowList.ItemTextFormatFlags = StringFormatFlags.NoWrap;
            this.FlowList.Dock = DockStyle.Fill;
            this.FlowList.IsLileList = false;
            this.FlowList.ItemSpace = 0;
            this.FlowList.IsMultiSelect = false;
            this.FlowList.Font = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);
            this.FlowList.SetItemSize(192, 32);
            this.FlowList.CanKeyDown = false;

            this.FlowList.ItemMouseClick += new(this.FlowList_ItemMouseClick);
            this.FlowList.DrawItem += new(this.FlowList_DrawItem);
        }

        public void SetItems(IList<string> itemList)
        {
            ArgumentNullException.ThrowIfNull(itemList, nameof(itemList));

            this.FlowList.BeginUpdate();

            try
            {
                this.itemList.Clear();
                this.itemList.AddRange(itemList);

                this.FlowList.ClearSelectedItems();
                this.FlowList.ItemCount = this.itemList.Count;
            }
            finally
            {
                this.FlowList.EndUpdate();
            }
        }

        public void AddItems(IList<string> itemList)
        {
            ArgumentNullException.ThrowIfNull(itemList, nameof(itemList));

            this.FlowList.BeginUpdate();

            try
            {
                this.itemList.AddRange(itemList);
                var tempItemList = this.itemList
                    .GroupBy(item => item)
                    .Select(item => item.First())
                    .OrderBy(item => item)
                    .ToList();
                this.itemList.Clear();
                this.itemList.AddRange(tempItemList);
                this.FlowList.ItemCount = this.itemList.Count;
            }
            finally
            {
                this.FlowList.EndUpdate();
            }
        }

        public void SelectItem(string item)
        {
            ArgumentException.ThrowIfNullOrEmpty(item, nameof(item));

            var index = this.itemList.IndexOf(item);
            if (index < 0)
            {
                return;
            }

            this.FlowList.BeginUpdate();

            try
            {
                this.FlowList.SelectItem(index);
            }
            finally
            {
                this.FlowList.EndUpdate();
            }
        }

        protected override void OnOpened(EventArgs e)
        {
            this.FlowList.Focus();
            base.OnOpened(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.FlowList.Invalidate();
            base.OnInvalidated(e);
        }

        private RectangleF GetIconRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.Icon != null)
            {
                return new RectangleF(e.ItemRectangle.X + this.Icon.Width / 4f,
                                      e.ItemRectangle.Y + this.Icon.Height / 2f,
                                      this.Icon.Width,
                                      this.Icon.Height);
            }
            else
            {
                return new RectangleF(e.ItemRectangle.X, e.ItemRectangle.Y, 0, 0);
            }
        }

        private RectangleF GetTextRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.Icon != null)
            {
                return new RectangleF(e.ItemRectangle.X + this.Icon.Width + this.Icon.Width / 2f,
                                      e.ItemRectangle.Bottom - this.ItemTextHeight,
                                      e.ItemRectangle.Width,
                                      this.ItemTextHeight);
            }
            else
            {
                return new RectangleF(e.ItemRectangle.X,
                                      e.ItemRectangle.Bottom - this.ItemTextHeight,
                                      e.ItemRectangle.Width,
                                      this.ItemTextHeight);
            }

        }

        private void FlowList_DrawItem(object sender, UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.itemList.Count < 1)
            {
                return;
            }

            if (this.Icon != null)
            {
                var rect = this.GetIconRectangle(e);
                e.Graphics.DrawImage(this.Icon, rect);
            }

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.FlowList.SelectedItemBrush, e.ItemRectangle);
            }

            if (e.IsFocus)
            {
                e.Graphics.FillRectangle(this.FlowList.FocusItemBrush, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.FlowList.MousePointItemBrush, e.ItemRectangle);
            }

            var item = this.itemList[e.ItemIndex];
            e.Graphics.DrawString(item, this.FlowList.Font, this.FlowList.ItemTextBrush, this.GetTextRectangle(e), this.FlowList.ItemTextFormat);
        }

        private void FlowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            if (this.IsClickAndClose)
            {
                this.Close();
            }

            if (this.itemList.Count < 1)
            {
                return;
            }

            var indexs = this.FlowList.GetSelectedIndexs();
            if (indexs.Count < 1)
            {
                return;
            }

            var index = indexs.First();
            if (this.itemList.Count - 1 < index)
            {
                return;
            }

            if (this.ItemMouseClick != null)
            {
                var item = this.itemList[indexs.First()];
                var args = new ItemMouseClickEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta, item);
                this.ItemMouseClick(this, args);
            }
        }

    }
}
