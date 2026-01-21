using SWF.UIComponent.FlowList;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    /// <summary>
    /// ドロップダウンリスト
    /// </summary>

    public sealed partial class DropDownList
        : ToolStripDropDown
    {
        public event EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs> Drawitem;
        public event EventHandler<MouseEventArgs> ItemMouseClick;
        public event EventHandler ItemExecute;

        private readonly FlowList _flowList;

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
        /// 項目数
        /// </summary>
        [Category("項目表示"), DefaultValue(0)]
        public int ItemCount
        {
            get
            {
                return this._flowList.ItemCount;
            }
            set
            {
                this._flowList.ItemCount = value;
            }
        }

        /// <summary>
        /// 項目高さ
        /// </summary>
        [Category("項目表示")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ItemHeight
        {
            get
            {
                return this._flowList.ItemHeight;
            }
            set
            {
                this._flowList.ItemHeight = value;
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
        /// サイズ
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        /// <summary>
        /// サイズ
        /// </summary>
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                this.ToolStripItem.Size = value;
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

        private ToolStripControlHost ToolStripItem
        {
            get
            {
                return (ToolStripControlHost)this.Items[0];
            }
        }

        public DropDownList()
        {
            this.DoubleBuffered = false;

            this._flowList = new();
            this.Items.Add(new ToolStripControlHost(this._flowList));
            this.Padding = new Padding(2, 1, 2, 0);

            this.ToolStripItem.AutoSize = false;
            this.ToolStripItem.BackColor = this.BackColor;

            this._flowList.Dock = DockStyle.Fill;
            this._flowList.IsLileList = true;
            this._flowList.ItemSpace = 0;
            this._flowList.IsMultiSelect = false;
            this._flowList.BackColor = this.BackColor;
            this._flowList.MouseWheelRate = 2.5f;

            this._flowList.DrawItem += new(this.FlowList_Drawitem);
            this._flowList.ItemExecute += new(this.FlowList_ItemExecute);
            this._flowList.ItemMouseClick += new(this.FlowList_ItemMouseClick);

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

        /// <summary>
        /// 描画を停止します。
        /// </summary>
        public void BeginUpdate()
        {
            this._flowList.BeginUpdate();
        }

        /// <summary>
        /// 描画を開始します。
        /// </summary>
        public void EndUpdate()
        {
            this._flowList.EndUpdate();
        }

        /// <summary>
        /// 選択項目をクリアします。
        /// </summary>
        public void ClearSelectedItems()
        {
            this._flowList.ClearSelectedItems();
        }

        /// <summary>
        /// 指定した項目を選択します。
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            this._flowList.SelectItem(itemIndex);
        }

        /// <summary>
        /// スクリーン座標から、項目のインデックスを取得します。
        /// </summary>
        /// <returns>座標上に項目が存在する場合は、項目のインデックス。存在しない場合は-1を返します。</returns>
        public int IndexFromScreenPoint()
        {
            var clientPoint = this._flowList.PointToClient(Cursor.Position);
            return this._flowList.IndexFromPoint(clientPoint.X, clientPoint.Y);
        }

        /// <summary>
        /// 選択している項目のインデックスを取得します。
        /// </summary>
        /// <returns></returns>
        public int[] GetSelectedIndexs()
        {
            return this._flowList.GetSelectedIndexs();
        }

        private void OnDrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            this.Drawitem?.Invoke(this, e);
        }

        private void OnItemMouseClick(MouseEventArgs e)
        {
            this.ItemMouseClick?.Invoke(this, e);
        }

        private void OnItemExecute(EventArgs e)
        {
            this.ItemExecute?.Invoke(this, e);
        }

        private void FlowList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            this.OnDrawItem(e);
        }

        private void FlowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            this.OnItemMouseClick(e);
        }

        private void FlowList_ItemExecute(object sender, EventArgs e)
        {
            this.OnItemExecute(e);
        }

    }
}
