using SWF.UIComponent.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class WideDropDownList
        : ToolStripDropDown
    {
        private static readonly Size BACKGROUND_DEFAULT_SIZE = new(866, 250);
        private static readonly Size ITEM_DEFAULT_SIZE = new(212, 32);

        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;

        private readonly Control _owner;
        private readonly List<string> _itemList = [];

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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
                return this.FlowList.GetScrollBarWidth();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Icon { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        public WideDropDownList(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            this._owner = owner;

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
            this.ToolStripItem.Size = new Size(BACKGROUND_DEFAULT_SIZE.Width, BACKGROUND_DEFAULT_SIZE.Height);

            this.FlowList.BackColor = Color.White;
            this.FlowList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            this.FlowList.ItemTextLineAlignment = StringAlignment.Center;
            this.FlowList.ItemTextAlignment = StringAlignment.Near;
            this.FlowList.ItemTextFormatFlags = StringFormatFlags.NoWrap;
            this.FlowList.Dock = DockStyle.Fill;
            this.FlowList.IsLileList = false;
            this.FlowList.ItemSpace = 0;
            this.FlowList.IsMultiSelect = false;
            this.FlowList.Font = new Font("Yu Gothic UI", 10F);
            this.FlowList.SetItemSize(ITEM_DEFAULT_SIZE.Width, ITEM_DEFAULT_SIZE.Height);
            this.FlowList.CanKeyDown = false;

            this.FlowList.ItemMouseClick += new(this.FlowList_ItemMouseClick);
            this.FlowList.DrawItem += new(this.FlowList_DrawItem);
        }

        public void SetItems(string[] itemList)
        {
            ArgumentNullException.ThrowIfNull(itemList, nameof(itemList));

            this.FlowList.BeginUpdate();

            try
            {
                this._itemList.Clear();
                this._itemList.AddRange(itemList);

                this.FlowList.ClearSelectedItems();
                this.FlowList.ItemCount = this._itemList.Count;
            }
            finally
            {
                this.FlowList.EndUpdate();
            }
        }

        public void SelectItem(string item)
        {
            ArgumentException.ThrowIfNullOrEmpty(item, nameof(item));

            var index = this._itemList.IndexOf(item);
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

        protected override void OnOpening(CancelEventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this._owner);
            this.ToolStripItem.Size = new(
                (int)(BACKGROUND_DEFAULT_SIZE.Width * scale),
                (int)(BACKGROUND_DEFAULT_SIZE.Height * scale));
            this.FlowList.SetItemSize(
                (int)(ITEM_DEFAULT_SIZE.Width * scale),
                (int)(ITEM_DEFAULT_SIZE.Height * scale));

            base.OnOpening(e);
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
                var scale = WindowUtil.GetCurrentWindowScale(this);
                var margin = 8 * scale;
                var size = Math.Min(this.Icon.Width, e.ItemRectangle.Height) - margin * 2;
                return new RectangleF(e.ItemRectangle.X + margin,
                                      e.ItemRectangle.Y + margin,
                                      size,
                                      size);
            }
            else
            {
                return new RectangleF(e.ItemRectangle.X, e.ItemRectangle.Y, 0, 0);
            }
        }

        private void FlowList_DrawItem(object sender, UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (this._itemList.Count < 1)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

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

            var iconWidth = Math.Min(this.Icon.Width, e.ItemRectangle.Height);
            var itemText = this._itemList[e.ItemIndex];
            var itemTextSize = TextRenderer.MeasureText(itemText, this.FlowList.Font);
            var destText = itemText;
            var destTextSize = itemTextSize;
            var itemWidth = e.ItemRectangle.Width - iconWidth;
            while (destTextSize.Width > itemWidth)
            {
                destText = destText.Substring(0, destText.Length - 1);
                destTextSize = TextRenderer.MeasureText($"{destText}...", this.FlowList.Font);
            }
            destText = itemText == destText ? itemText : $"{destText}...";

            var textRect = new Rectangle(e.ItemRectangle.X + iconWidth,
                                         e.ItemRectangle.Y + (int)((e.ItemRectangle.Height - destTextSize.Height) / 2f),
                                         e.ItemRectangle.Width - iconWidth,
                                         e.ItemRectangle.Height);

            TextRenderer.DrawText(
                e.Graphics,
                destText,
                this.FlowList.Font,
                textRect.Location,
                this.FlowList.ItemTextBrush.Color,
                TextFormatFlags.Top);
        }

        private void FlowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            if (this.IsClickAndClose)
            {
                this.Close();
            }

            if (this._itemList.Count < 1)
            {
                return;
            }

            var indexs = this.FlowList.GetSelectedIndexs();
            if (indexs.Length < 1)
            {
                return;
            }

            var index = indexs.First();
            if (this._itemList.Count - 1 < index)
            {
                return;
            }

            if (this.ItemMouseClick != null)
            {
                var item = this._itemList[indexs.First()];
                var args = new ItemMouseClickEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta, item);
                this.ItemMouseClick(this, args);
            }
        }

    }
}
