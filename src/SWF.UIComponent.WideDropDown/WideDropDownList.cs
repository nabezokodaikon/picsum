using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    public sealed class WideDropDownList
        : ToolStripDropDown
    {
        #region イベント・デリゲート

        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;

        #endregion

        private readonly List<string> itemList = new List<string>();

        #region パブリックプロパティ

        /// <summary>
        /// 項目テキスト色
        /// </summary>
        [Category("項目描画")]
        public Color ItemTextColor
        {
            get
            {
                return this.flowList.ItemTextColor;
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
                return this.flowList.SelectedItemColor;
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
                return this.flowList.MousePointItemColor;
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
                return this.flowList.ItemTextTrimming;
            }
            set
            {
                this.flowList.ItemTextTrimming = value;
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
                return this.flowList.ItemTextAlignment;
            }
            set
            {
                this.flowList.ItemTextAlignment = value;
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
                return this.flowList.ItemTextLineAlignment;
            }
            set
            {
                this.flowList.ItemTextLineAlignment = value;
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
                return this.flowList.ItemTextFormatFlags;
            }
            set
            {
                this.flowList.ItemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public StringFormat ItemTextFormat
        {
            get
            {
                return this.flowList.ItemTextFormat;
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
                this.toolStripItem.BackColor = value;
                this.flowList.BackColor = value;
            }
        }

        /// <summary>
        /// スクロールバーの幅
        /// </summary>
        public int ScrollBarWidth
        {
            get
            {
                return this.flowList.ScrollBarWidth;
            }
        }

        public Image Icon { get; set; }

        internal bool IsClickAndClose { get; set; } 

        #endregion

        #region プライベートプロパティ

        private ToolStripControlHost toolStripItem
        {
            get
            {
                return (ToolStripControlHost)this.Items[0];
            }
        }

        private FlowList.FlowList flowList
        {
            get
            {
                return (FlowList.FlowList)((ToolStripControlHost)this.Items[0]).Control;
            }
        }

        private int itemTextHeight
        {
            get
            {
                return this.FontHeight * 2;
            }
        }

        #endregion

        #region コンストラクタ

        public WideDropDownList()
        {
            this.SetStyle(ControlStyles.Selectable, false);

            this.Items.Add(new ToolStripControlHost(new FlowList.FlowList()));
            this.Padding = new Padding(2, 1, 2, 0);

            this.toolStripItem.AutoSize = false;
            this.toolStripItem.BackColor = this.BackColor;
            this.toolStripItem.Size = new Size(456, 224);

            this.flowList.BackColor = Color.White;
            this.flowList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            this.flowList.ItemTextLineAlignment = StringAlignment.Center;
            this.flowList.ItemTextAlignment = StringAlignment.Near;
            this.flowList.ItemTextFormatFlags = StringFormatFlags.NoWrap;
            this.flowList.Dock = DockStyle.Fill;
            this.flowList.IsLileList = false;
            this.flowList.ItemSpace = 0;
            this.flowList.IsMultiSelect = false;
            this.flowList.Font = new Font("Yu Gothic UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);
            this.flowList.SetItemSize(144, 32);
            this.flowList.CanKeyDown = false;

            this.flowList.ItemMouseClick += new EventHandler<MouseEventArgs>(this.flowList_ItemMouseClick);
            this.flowList.DrawItem += new EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(this.flowList_DrawItem);
        }

        #endregion

        #region パブリックメソッド

        public void SetItems(IList<string> itemList)
        {
            if (itemList == null)
            {
                throw new ArgumentNullException(nameof(itemList));
            }

            this.flowList.BeginUpdate();

            try
            {
                this.itemList.Clear();
                this.itemList.AddRange(itemList);

                this.flowList.ClearSelectedItems();
                this.flowList.ItemCount = this.itemList.Count;
            }
            finally
            {
                this.flowList.EndUpdate();
            }
        }

        public void AddItems(IList<string> itemList)
        {
            if (itemList == null)
            {
                throw new ArgumentNullException(nameof(itemList));
            }

            this.flowList.BeginUpdate();

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
                this.flowList.ItemCount = this.itemList.Count;
            }
            finally
            {
                this.flowList.EndUpdate();
            }
        }

        public void SelectItem(string item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = this.itemList.IndexOf(item);
            if (index < 0)
            {
                return;
            }

            this.flowList.BeginUpdate();

            try
            {
                this.flowList.SelectItem(index);
            }
            finally
            {
                this.flowList.EndUpdate();
            }
        }

        #endregion

        #region 継承メソッド

        protected override void OnOpened(EventArgs e)
        {
            this.flowList.Focus();
            base.OnOpened(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            this.flowList.Invalidate();
            base.OnInvalidated(e);
        }

        #endregion

        #region プライベートメソッド

        private Rectangle getIconRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.Icon != null)
            {
                return new Rectangle(e.ItemRectangle.X + this.Icon.Width / 4,
                                     e.ItemRectangle.Y + this.Icon.Height / 2,
                                     this.Icon.Width,
                                     this.Icon.Height);
            }
            else
            {
                return new Rectangle(e.ItemRectangle.X, e.ItemRectangle.Y, 0, 0);
            }
        }

        private Rectangle getTextRectangle(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.Icon != null)
            {
                return new Rectangle(e.ItemRectangle.X + this.Icon.Width + this.Icon.Width / 2,
                                     e.ItemRectangle.Bottom - itemTextHeight,
                                     e.ItemRectangle.Width,
                                     itemTextHeight);
            }
            else 
            {
                return new Rectangle(e.ItemRectangle.X,
                                     e.ItemRectangle.Bottom - itemTextHeight,
                                     e.ItemRectangle.Width,
                                     itemTextHeight);
            }

        }

        private void flowList_DrawItem(object sender, UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this.itemList.Count < 1)
            {
                return;
            }

            if (this.Icon != null)
            {
                var rect = this.getIconRectangle(e);
                e.Graphics.DrawImage(this.Icon, rect);
            }

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this.flowList.SelectedItemBrush, e.ItemRectangle);
                e.Graphics.DrawRectangle(this.flowList.SelectedItemPen, e.ItemRectangle);
            }

            if (e.IsFocus)
            {
                e.Graphics.FillRectangle(this.flowList.FocusItemBrush, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this.flowList.MousePointItemBrush, e.ItemRectangle);
            }

            var item = this.itemList[e.ItemIndex];
            e.Graphics.DrawString(item, this.flowList.Font, this.flowList.ItemTextBrush, getTextRectangle(e), this.flowList.ItemTextFormat);
        }

        private void flowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            if (this.IsClickAndClose) 
            {
                this.Close();
            }            

            if (this.itemList.Count < 1)
            {
                return;
            }

            var indexs = this.flowList.GetSelectedIndexs();
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

        #endregion
    }
}
