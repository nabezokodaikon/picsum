using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWF.UIComponent.FlowList;

namespace PicSum.UIComponent.Common
{
    /// <summary>
    /// ドロップダウンリスト
    /// </summary>
    public class DropDownList : ToolStripDropDown
    {
        #region イベント・デリゲート

        public event EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs> Drawitem;
        public event EventHandler<MouseEventArgs> ItemMouseClick;
        public event EventHandler ItemExecute;

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 項目数
        /// </summary>
        [Category("項目表示"), DefaultValue(0)]
        public int ItemCount
        {
            get
            {
                return flowList.ItemCount;
            }
            set
            {
                flowList.ItemCount = value;
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
                return flowList.ItemHeight;
            }
            set
            {
                flowList.ItemHeight = value;
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
                return flowList.ItemTextColor;
            }
            set
            {
                flowList.ItemTextColor = value;
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
                return flowList.SelectedItemColor;
            }
            set
            {
                flowList.SelectedItemColor = value;
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
                return flowList.FocusItemColor;
            }
            set
            {
                flowList.FocusItemColor = value;
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
                return flowList.MousePointItemColor;
            }
            set
            {
                flowList.MousePointItemColor = value;
            }
        }

        /// <summary>
        /// 短形選択色
        /// </summary>
        [Category("項目描画")]
        public Color RectangleSelectionColor
        {
            get
            {
                return flowList.RectangleSelectionColor;
            }
            set
            {
                flowList.RectangleSelectionColor = value;
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
                return flowList.ItemTextTrimming;
            }
            set
            {
                flowList.ItemTextTrimming = value;
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
                return flowList.ItemTextAlignment;
            }
            set
            {
                flowList.ItemTextAlignment = value;
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
                return flowList.ItemTextLineAlignment;
            }
            set
            {
                flowList.ItemTextLineAlignment = value;
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
                return flowList.ItemTextFormatFlags;
            }
            set
            {
                flowList.ItemTextFormatFlags = value;
            }
        }

        [Browsable(false)]
        public SolidBrush ItemTextBrush
        {
            get
            {
                return flowList.ItemTextBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush SelectedItemBrush
        {
            get
            {
                return flowList.SelectedItemBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush FocusItemBrush
        {
            get
            {
                return flowList.FocusItemBrush;
            }
        }

        [Browsable(false)]
        public SolidBrush MousePointItemBrush
        {
            get
            {
                return flowList.MousePointItemBrush;
            }
        }

        [Browsable(false)]
        public StringFormat ItemTextFormat
        {
            get
            {
                return flowList.ItemTextFormat;
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
                toolStripItem.BackColor = value;
                flowList.BackColor = value;
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
                toolStripItem.Size = value;
            }
        }

        /// <summary>
        /// スクロールバーの幅
        /// </summary>
        public int ScrollBarWidth
        {
            get
            {
                return flowList.ScrollBarWidth;
            }
        }

        #endregion

        #region プライベートプロパティ

        private ToolStripControlHost toolStripItem
        {
            get
            {
                return (ToolStripControlHost)this.Items[0];
            }
        }

        private FlowList flowList
        {
            get
            {
                return (FlowList)toolStripItem.Control;
            }
        }

        #endregion

        #region コンストラクタ

        public DropDownList()
        {
            initializeComponent();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 描画を停止します。
        /// </summary>
        public void BeginUpdate()
        {
            flowList.BeginUpdate();
        }

        /// <summary>
        /// 描画を開始します。
        /// </summary>
        public void EndUpdate()
        {
            flowList.EndUpdate();
        }

        /// <summary>
        /// 選択項目をクリアします。
        /// </summary>
        public void ClearSelectedItems()
        {
            flowList.ClearSelectedItems();
        }

        /// <summary>
        /// 指定した項目を選択します。
        /// </summary>
        /// <param name="itemIndex"></param>
        public void SelectItem(int itemIndex)
        {
            flowList.SelectItem(itemIndex);
        }

        /// <summary>
        /// スクリーン座標から、項目のインデックスを取得します。
        /// </summary>
        /// <returns>座標上に項目が存在する場合は、項目のインデックス。存在しない場合は-1を返します。</returns>
        public int IndexFromScreenPoint()
        {
            Point clientPoint = flowList.PointToClient(Cursor.Position);
            return flowList.IndexFromPoint(clientPoint.X, clientPoint.Y);
        }

        /// <summary>
        /// 選択している項目のインデックスを取得します。
        /// </summary>
        /// <returns></returns>
        public IList<int> GetSelectedIndexs()
        {
            return flowList.GetSelectedIndexs();
        }

        #endregion

        #region 継承メソッド

        protected override void OnOpened(EventArgs e)
        {
            flowList.Focus();

            base.OnOpened(e);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            flowList.Invalidate();
            base.OnInvalidated(e);
        }

        protected virtual void OnDrawItem(SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            if (Drawitem != null)
            {
                Drawitem(this, e);
            }
        }

        protected virtual void OnItemMouseClick(MouseEventArgs e)
        {
            if (ItemMouseClick != null)
            {
                ItemMouseClick(this, e);
            }
        }

        protected virtual void OnItemExecute(EventArgs e)
        {
            if (ItemExecute != null)
            {
                ItemExecute(this, e);
            }
        }

        #endregion

        #region プライベートメソッド

        private void initializeComponent()
        {
            this.SetStyle(ControlStyles.Selectable, false);

            this.Items.Add(new ToolStripControlHost(new FlowList()));
            this.Padding = new Padding(2, 1, 2, 0);

            toolStripItem.AutoSize = false;
            toolStripItem.BackColor = this.BackColor;

            flowList.Dock = DockStyle.Fill;
            flowList.IsLileList = true;
            flowList.ItemSpace = 0;
            flowList.IsMultiSelect = false;
            flowList.BackColor = this.BackColor;

            flowList.DrawItem += new EventHandler<SWF.UIComponent.FlowList.DrawItemEventArgs>(flowList_Drawitem);
            flowList.ItemExecute += new EventHandler(flowList_ItemExecute);
            flowList.ItemMouseClick += new EventHandler<MouseEventArgs>(flowList_ItemMouseClick);
        }

        #endregion

        #region フローリストイベント

        private void flowList_Drawitem(object sender, SWF.UIComponent.FlowList.DrawItemEventArgs e)
        {
            OnDrawItem(e);
        }

        private void flowList_ItemMouseClick(object sender, MouseEventArgs e)
        {
            OnItemMouseClick(e);
        }

        private void flowList_ItemExecute(object sender, EventArgs e)
        {
            OnItemExecute(e);
        }

        #endregion
    }
}
