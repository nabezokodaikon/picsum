using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SWF.UIComponent.WideDropDown
{

    public sealed partial class WideDropDownList
        : ToolStripDropDown
    {
        private static readonly Size BACKGROUND_DEFAULT_SIZE = new(866, 250);
        private static readonly Size ITEM_DEFAULT_SIZE = new(212, 32);

        public event EventHandler<ItemMouseClickEventArgs> ItemMouseClick;

        private readonly Control _owner;
        private readonly List<string> _itemList = [];
        private readonly FlowList.FlowList _flowList;

        public new string Name
        {
            get
            {
                return base.Name;
            }
            private set
            {
                base.Name = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            private set
            {
                base.DoubleBuffered = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            private set
            {
                base.TabIndex = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            private set
            {
                base.TabStop = value;
            }
        }

        /// <summary>
        /// 項目テキスト色
        /// </summary>
        [Category("項目描画")]
        public Color ItemTextColor
        {
            get
            {
                return this._flowList.ItemTextColor;
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
                return this._flowList.SelectedItemColor;
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
                return this._flowList.MousePointItemColor;
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
                return this._flowList.ItemTextTrimming;
            }
            set
            {
                this._flowList.ItemTextTrimming = value;
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
                return this._flowList.ItemTextAlignment;
            }
            set
            {
                this._flowList.ItemTextAlignment = value;
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
                return this._flowList.ItemTextLineAlignment;
            }
            set
            {
                this._flowList.ItemTextLineAlignment = value;
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
                return this._flowList.ItemTextFormatFlags;
            }
            set
            {
                this._flowList.ItemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public StringFormat ItemTextFormat
        {
            get
            {
                return this._flowList.ItemTextFormat;
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
                this._flowList.BackColor = value;
            }
        }

        /// <summary>
        /// スクロールバーの幅
        /// </summary>
        public int ScrollBarWidth
        {
            get
            {
                return this._flowList.GetScrollBarWidth();
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

        public WideDropDownList(Control owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            this._owner = owner;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl |
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();

            this.DoubleBuffered = true;

            this._flowList = new FlowList.FlowList();
            this.Items.Add(new ToolStripControlHost(this._flowList));
            this.Padding = new Padding(2, 1, 2, 0);

            this.ToolStripItem.AutoSize = false;
            this.ToolStripItem.BackColor = this.BackColor;
            this.ToolStripItem.Size = new Size(BACKGROUND_DEFAULT_SIZE.Width, BACKGROUND_DEFAULT_SIZE.Height);

            this._flowList.BackColor = Color.White;
            this._flowList.ItemTextTrimming = StringTrimming.EllipsisCharacter;
            this._flowList.ItemTextLineAlignment = StringAlignment.Center;
            this._flowList.ItemTextAlignment = StringAlignment.Near;
            this._flowList.ItemTextFormatFlags = StringFormatFlags.NoWrap;
            this._flowList.Dock = DockStyle.Fill;
            this._flowList.IsLileList = false;
            this._flowList.ItemSpace = 0;
            this._flowList.IsMultiSelect = false;
            this._flowList.SetItemSize(ITEM_DEFAULT_SIZE.Width, ITEM_DEFAULT_SIZE.Height);
            this._flowList.CanKeyDown = false;
            this._flowList.MouseWheelRate = 2.5f;

            this._flowList.ItemMouseClick += new(this.FlowList_ItemMouseClick);
            this._flowList.DrawItem += new(this.FlowList_DrawItem);

            this.Opening += this.WideDropDownList_Opening;
            this.Invalidated += this.WideDropDownList_Invalidated;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._flowList.Dispose();
            }

            base.Dispose(disposing);
        }

        public void SetItems(string[] itemList)
        {
            ArgumentNullException.ThrowIfNull(itemList, nameof(itemList));

            this._flowList.BeginUpdate();

            try
            {
                this._itemList.Clear();
                this._itemList.AddRange(itemList);

                this._flowList.ClearSelectedItems();
                this._flowList.ItemCount = this._itemList.Count;
            }
            finally
            {
                this._flowList.EndUpdate();
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

            this._flowList.BeginUpdate();

            try
            {
                this._flowList.SelectItem(index);
            }
            finally
            {
                this._flowList.EndUpdate();
            }
        }

        private void WideDropDownList_Opening(object sender, CancelEventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this._owner);
            this.ToolStripItem.Size = new(
                (int)(BACKGROUND_DEFAULT_SIZE.Width * scale),
                (int)(BACKGROUND_DEFAULT_SIZE.Height * scale));
            this._flowList.SetItemSize(
                (int)(ITEM_DEFAULT_SIZE.Width * scale),
                (int)(ITEM_DEFAULT_SIZE.Height * scale));
        }

        private void WideDropDownList_Invalidated(object sender, InvalidateEventArgs e)
        {
            this._flowList.Invalidate();
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
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            if (this.Icon != null)
            {
                var rect = this.GetIconRectangle(e);
                e.Graphics.DrawImage(this.Icon, rect);
            }

            if (e.IsSelected)
            {
                e.Graphics.FillRectangle(this._flowList.SelectedItemBrush, e.ItemRectangle);
            }

            if (e.IsFocus)
            {
                e.Graphics.FillRectangle(this._flowList.FocusItemBrush, e.ItemRectangle);
            }

            if (e.IsMousePoint)
            {
                e.Graphics.FillRectangle(this._flowList.MousePointItemBrush, e.ItemRectangle);
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            var iconWidth = Math.Min(this.Icon.Width, e.ItemRectangle.Height);
            var itemText = this._itemList[e.ItemIndex];
            var itemTextSize = TextRenderer.MeasureText(itemText, font);
            var destText = itemText;
            var destTextSize = itemTextSize;
            var itemWidth = e.ItemRectangle.Width - iconWidth;
            while (destTextSize.Width > itemWidth)
            {
                destText = destText[..^1];
                destTextSize = TextRenderer.MeasureText($"{destText}...", font);
            }
            destText = itemText == destText ? itemText : $"{destText}...";

            var textRect = new Rectangle(e.ItemRectangle.X + iconWidth,
                                         e.ItemRectangle.Y + (int)((e.ItemRectangle.Height - destTextSize.Height) / 2f),
                                         e.ItemRectangle.Width - iconWidth,
                                         e.ItemRectangle.Height);

            TextRenderer.DrawText(
                e.Graphics,
                destText,
                font,
                textRect.Location,
                this._flowList.ItemTextBrush.Color,
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

            var indexs = this._flowList.GetSelectedIndexs();
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
