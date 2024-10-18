using SWF.UIComponent.FlowList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.AddressBar
{
    /// <summary>
    /// ドロップダウンリスト
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed partial class DropDownList
        : ToolStripDropDown
    {

        public event EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs> Drawitem;
        public event EventHandler<MouseEventArgs> ItemMouseClick;
        public event EventHandler ItemExecute;

        /// <summary>
        /// 項目数
        /// </summary>
        [Category("項目表示"), DefaultValue(0)]
        public int ItemCount
        {
            get
            {
                return this.FlowList.ItemCount;
            }
            set
            {
                this.FlowList.ItemCount = value;
            }
        }

        /// <summary>
        /// 項目高さ
        /// </summary>
        [Category("項目表示")]
        public int ItemHeight
        {
            get
            {
                return this.FlowList.ItemHeight;
            }
            set
            {
                this.FlowList.ItemHeight = value;
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
        /// 項目フォーカス色
        /// </summary>
        [Category("項目描画")]
        public Color FocusItemColor
        {
            get
            {
                return this.FlowList.FocusItemColor;
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
        public SolidBrush ItemTextBrush
        {
            get
            {
                return this.FlowList.ItemTextBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush SelectedItemBrush
        {
            get
            {
                return this.FlowList.SelectedItemBrush;
            }
        }

        [Browsable(false)]
        public Pen SelectedItemPen
        {
            get
            {
                return this.FlowList.SelectedItemPen;
            }
        }

        [Browsable(false)]
        public SolidBrush FocusItemBrush
        {
            get
            {
                return this.FlowList.FocusItemBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush MousePointItemBrush
        {
            get
            {
                return this.FlowList.MousePointItemBrush;
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
                return this.FlowList.ScrollBarWidth;
            }
        }

        private ToolStripControlHost ToolStripItem
        {
            get
            {
                return (ToolStripControlHost)this.Items[0];
            }
        }

        private FlowList FlowList
        {
            get
            {
                return (FlowList)this.ToolStripItem.Control;
            }
        }

        public DropDownList()
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

            this.Items.Add(new ToolStripControlHost(new FlowList()));
            this.Padding = new Padding(2, 1, 2, 0);

            this.ToolStripItem.AutoSize = false;
            this.ToolStripItem.BackColor = this.BackColor;

            this.FlowList.Dock = DockStyle.Fill;
            this.FlowList.IsLileList = true;
            this.FlowList.ItemSpace = 0;
            this.FlowList.IsMultiSelect = false;
            this.FlowList.BackColor = this.BackColor;

            this.FlowList.DrawItem += new(this.FlowList_Drawitem);
            this.FlowList.ItemExecute += new(this.FlowList_ItemExecute);
            this.FlowList.ItemMouseClick += new(this.FlowList_ItemMouseClick);
        }

        /// <summary>
        /// 描画を停止します。
        /// </summary>
        public void BeginUpdate()
        {
            this.FlowList.BeginUpdate();
        }

        /// <summary>
        /// 描画を開始します。
        /// </summary>
        public void EndUpdate()
        {
            this.FlowList.EndUpdate();
        }

        /// <summary>
        /// 選択項目をクリアします。
        /// </summary>
        public void ClearSelectedItems()
        {
            this.FlowList.ClearSelectedItems();
        }

        /// <summary>
        /// 指定した項目を選択します。
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            this.FlowList.SelectItem(itemIndex);
        }

        /// <summary>
        /// スクリーン座標から、項目のインデックスを取得します。
        /// </summary>
        /// <returns>座標上に項目が存在する場合は、項目のインデックス。存在しない場合は-1を返します。</returns>
        public int IndexFromScreenPoint()
        {
            var clientPoint = this.FlowList.PointToClient(Cursor.Position);
            return this.FlowList.IndexFromPoint(clientPoint.X, clientPoint.Y);
        }

        /// <summary>
        /// 選択している項目のインデックスを取得します。
        /// </summary>
        /// <returns></returns>
        public IList<int> GetSelectedIndexs()
        {
            return this.FlowList.GetSelectedIndexs();
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
