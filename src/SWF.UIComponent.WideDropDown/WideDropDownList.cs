using SkiaSharp;
using SWF.Core.Base;
using SWF.Core.ImageAccessor;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.FlowList;
using SWF.UIComponent.SKFlowList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        private readonly SKFlowList.SKFlowList _flowList;

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
        public SKImage Icon { get; set; }

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

            this.DoubleBuffered = false;

            this._flowList = new SKFlowList.SKFlowList();
            this.Items.Add(new ToolStripControlHost(this._flowList));
            this.Padding = new Padding(2, 1, 2, 0);

            this.ToolStripItem.AutoSize = false;
            this.ToolStripItem.BackColor = this.BackColor;
            this.ToolStripItem.Size = new Size(BACKGROUND_DEFAULT_SIZE.Width, BACKGROUND_DEFAULT_SIZE.Height);

            this._flowList.BackColor = Color.White;
            this._flowList.Dock = DockStyle.Fill;
            this._flowList.IsLileList = false;
            this._flowList.ItemSpace = 0;
            this._flowList.IsMultiSelect = false;
            this._flowList.SetItemSize(ITEM_DEFAULT_SIZE.Width, ITEM_DEFAULT_SIZE.Height);
            this._flowList.CanKeyDown = false;
            this._flowList.MouseWheelRate = 2.5f;

            this._flowList.ItemMouseClick += new(this.FlowList_ItemMouseClick);
            this._flowList.SKDrawItem += new(this.FlowList_DrawItem);

            this.Opening += this.WideDropDownList_Opening;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.SetStyle(
                ControlStyles.ContainerControl |
                ControlStyles.Selectable,
                false);
            this.UpdateStyles();
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

        public new void Invalidate(Rectangle rc)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._flowList.Invalidate(this._flowList.ClientRectangle);

            base.Invalidate(rc);
        }

        public new void Invalidate()
        {
            this.Invalidate(this.ClientRectangle);
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

        private SKRect GetIconRectangle(SKDrawItemEventArgs e)
        {
            if (this.Icon != null)
            {
                var scale = WindowUtil.GetCurrentWindowScale(this);
                var margin = 8 * scale;
                var size = Math.Min(this.Icon.Width, e.ItemRectangle.Height) - margin * 2;
                return SKRect.Create(e.ItemRectangle.Left + margin,
                                      e.ItemRectangle.Top + margin,
                                      size,
                                      size);
            }
            else
            {
                return SKRect.Create(e.ItemRectangle.Left, e.ItemRectangle.Top, 0, 0);
            }
        }

        private void FlowList_DrawItem(object sender, SKDrawItemEventArgs e)
        {
            if (this._itemList.Count < 1)
            {
                return;
            }

            if (this.Icon != null)
            {
                var rect = this.GetIconRectangle(e);
                e.Canvas.DrawImage(this.Icon, rect);
            }

            if (e.IsSelected && e.IsMousePoint)
            {
                e.Canvas.DrawRect(e.ItemRectangle, SKFlowListResouces.LIGHT_SELECTED_FILL_PAINT);
            }
            else if (e.IsSelected)
            {
                e.Canvas.DrawRect(e.ItemRectangle, SKFlowListResouces.LIGHT_SELECTED_FILL_PAINT);
            }
            else if (e.IsMousePoint)
            {
                e.Canvas.DrawRect(e.ItemRectangle, SKFlowListResouces.LIGHT_MOUSE_POINT_FILL_PAINT);
            }

            var iconWidth = Math.Min(this.Icon.Width, e.ItemRectangle.Height);

            var textRect = SKRect.Create(
                e.ItemRectangle.Left + iconWidth,
                e.ItemRectangle.Top,
                e.ItemRectangle.Width - iconWidth,
                e.ItemRectangle.Height);

            var font = FontCacher.GetRegularSKFont(
                FontCacher.Size.Medium,
                WindowUtil.GetCurrentWindowScale(this));

            SkiaUtil.DrawText(
                e.Canvas,
                SKFlowListResouces.LIGHT_TEXT_PAINT,
                font,
                this._itemList[e.ItemIndex],
                textRect,
                SKTextAlign.Left,
                1);
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
