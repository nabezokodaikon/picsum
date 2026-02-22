using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageView
{
    partial class ImageViewToolBar
    {
        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this._viewMenu = new System.Windows.Forms.ContextMenuStrip();
            this._singleViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._spreadLeftFeedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._spreadRightFeedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._doublePreviewButton = new SWF.UIComponent.Base.BaseTextButton();
            this._singlePreviewButton = new SWF.UIComponent.Base.BaseTextButton();
            this._doubleNextButton = new SWF.UIComponent.Base.BaseTextButton();
            this._singleNextButton = new SWF.UIComponent.Base.BaseTextButton();
            this._indexSlider = new SWF.UIComponent.Base.Slider();
            this._viewButton = new SWF.UIComponent.Base.BaseTextButton();
            this._sizeButton = new SWF.UIComponent.Base.BaseTextButton();
            this._sizeMenu = new System.Windows.Forms.ContextMenuStrip();
            this._originalSizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._fitWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._fitWindowLargeOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this._sizeMenuSeparator = new ToolStripSeparator();
            this._zoomMenuItem01 = new ZoomMenuItem(0.1f, "10%");
            this._zoomMenuItem02 = new ZoomMenuItem(0.25f, "25%");
            this._zoomMenuItem03 = new ZoomMenuItem(0.33f, "33%");
            this._zoomMenuItem04 = new ZoomMenuItem(0.5f, "50%");
            this._zoomMenuItem05 = new ZoomMenuItem(0.66f, "66%");
            this._zoomMenuItem06 = new ZoomMenuItem(0.75f, "75%");
            this._zoomMenuItem07 = new ZoomMenuItem(AppConstants.DEFAULT_ZOOM_VALUE, "100%");
            this._zoomMenuItem08 = new ZoomMenuItem(1.25f, "125%");
            this._zoomMenuItem09 = new ZoomMenuItem(1.5f, "150%");
            this._zoomMenuItem10 = new ZoomMenuItem(2f, "200%");
            this._zoomMenuItem11 = new ZoomMenuItem(3f, "300%");
            this._zoomMenuItem12 = new ZoomMenuItem(5f, "500%");
            this._zoomMenuItem13 = new ZoomMenuItem(10f, "1000%");
            this._zoomMenuItem14 = new ZoomMenuItem(20f, "2000%");
            this._zoomMenuItem15 = new ZoomMenuItem(30f, "3000%");

            this.filePathToolTip = new System.Windows.Forms.ToolTip();
            this._viewMenu.SuspendLayout();
            this._sizeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewMenu
            //
            this._viewMenu.Font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium);
            this._viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this._singleViewMenuItem, this._spreadLeftFeedMenuItem, this._spreadRightFeedMenuItem });
            // 
            // singleViewMenuItem
            // 
            this._singleViewMenuItem.Text = "Single View";
            this._singleViewMenuItem.Click += this.SingleViewMenuItem_Click;
            // 
            // spreadLeftFeedMenuItem
            // 
            this._spreadLeftFeedMenuItem.Text = "Spread (Left Feed)";
            this._spreadLeftFeedMenuItem.Click += this.SpreadLeftFeedMenuItem_Click;
            // 
            // spreadRightFeedMenuItem
            // 
            this._spreadRightFeedMenuItem.Text = "Spread (Right Feed)";
            this._spreadRightFeedMenuItem.Click += this.SpreadRightFeedMenuItem_Click;
            // 
            // doublePreviewButton
            //
            this._doublePreviewButton.Text = "<<-";
            this._doublePreviewButton.MouseClick += this.DoublePreviewButton_MouseClick;
            // 
            // singlePreviewButton
            //
            this._singlePreviewButton.Text = "<-";
            this._singlePreviewButton.MouseClick += this.SinglePreviewButton_MouseClick;
            // 
            // doubleNextButton
            //
            this._doubleNextButton.Text = "->>";
            this._doubleNextButton.MouseClick += this.DoubleNextButton_MouseClick;
            // 
            // singleNextButton
            //
            this._singleNextButton.Text = "->";
            this._singleNextButton.MouseClick += this.SingleNextButton_MouseClick;
            // 
            // indexSlider
            // 
            this._indexSlider.BeginValueChange += this.IndexSlider_BeginValueChange;
            this._indexSlider.ValueChanging += this.IndexSlider_ValueChanging;
            this._indexSlider.ValueChanged += this.IndexSlider_ValueChanged;
            this._indexSlider.MouseUp += this.IndexSlider_MouseUp;
            // 
            // viewButton
            //
            this._viewButton.Text = "View";
            this._viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // sizeButton
            //
            this._sizeButton.Text = "Size";
            this._sizeButton.MouseClick += this.SizeButton_MouseClick;
            // 
            // sizeMenu
            //
            this._sizeMenu.Font = FontCacher.GetRegularGdiFont(FontCacher.Size.Medium);
            this._sizeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this._fitWindowMenuItem,
                this._fitWindowLargeOnlyMenuItem,
                this._sizeMenuSeparator,
                this._zoomMenuItem01,
                this._zoomMenuItem02,
                this._zoomMenuItem03,
                this._zoomMenuItem04,
                this._zoomMenuItem05,
                this._zoomMenuItem06,
                //this.zoomMenuItem07,
                this._originalSizeMenuItem,
                this._zoomMenuItem08,
                this._zoomMenuItem09,
                this._zoomMenuItem10,
                this._zoomMenuItem11,
                this._zoomMenuItem12,
                this._zoomMenuItem13,
                this._zoomMenuItem14,
                this._zoomMenuItem15,
            });
            // 
            // originalSizeMenuItem
            // 
            this._originalSizeMenuItem.Text = "100%";
            this._originalSizeMenuItem.Click += this.OriginalSizeMenuItem_Click;
            // 
            // fitWindowMenuItem
            // 
            this._fitWindowMenuItem.Text = "Fit To Window";
            this._fitWindowMenuItem.Click += this.FitWindowMenuItem_Click;
            // 
            // fitWindowLargeOnlyMenuItem
            // 
            this._fitWindowLargeOnlyMenuItem.Text = "Fit To Window (Large Image Only)";
            this._fitWindowLargeOnlyMenuItem.Click += this.FitWindowLargeOnlyMenuItem_Click;
            // 
            // zoomMenuItem
            // 
            this._zoomMenuItem01.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem02.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem03.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem04.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem05.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem06.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem07.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem08.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem09.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem10.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem11.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem12.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem13.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem14.Click += this.ZoomMenuItem_Click;
            this._zoomMenuItem15.Click += this.ZoomMenuItem_Click;
            // 
            // filePathToolTip
            // 
            this.filePathToolTip.AutoPopDelay = 5000;
            this.filePathToolTip.BackColor = System.Drawing.SystemColors.Window;
            this.filePathToolTip.InitialDelay = 50;
            this.filePathToolTip.ReshowDelay = 100;
            // 
            // ImageViewToolBar
            //
            this.Controls.AddRange(
                this._sizeButton,
                this._viewButton,
                this._indexSlider,
                this._singleNextButton,
                this._doubleNextButton,
                this._singlePreviewButton,
                this._doublePreviewButton);
            this._viewMenu.ResumeLayout(false);
            this._sizeMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ToolStripDropDown _viewMenu;
        private SWF.UIComponent.Base.BaseTextButton _doublePreviewButton;
        private SWF.UIComponent.Base.BaseTextButton _singlePreviewButton;
        private SWF.UIComponent.Base.BaseTextButton _doubleNextButton;
        private SWF.UIComponent.Base.BaseTextButton _singleNextButton;
        private SWF.UIComponent.Base.Slider _indexSlider;
        private SWF.UIComponent.Base.BaseTextButton _viewButton;
        private System.Windows.Forms.ToolStripMenuItem _singleViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _spreadLeftFeedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _spreadRightFeedMenuItem;
        private SWF.UIComponent.Base.BaseTextButton _sizeButton;
        private System.Windows.Forms.ToolStripDropDown _sizeMenu;
        private System.Windows.Forms.ToolStripMenuItem _originalSizeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _fitWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _fitWindowLargeOnlyMenuItem;

        private System.Windows.Forms.ToolStripSeparator _sizeMenuSeparator;
        private ZoomMenuItem _zoomMenuItem01;
        private ZoomMenuItem _zoomMenuItem02;
        private ZoomMenuItem _zoomMenuItem03;
        private ZoomMenuItem _zoomMenuItem04;
        private ZoomMenuItem _zoomMenuItem05;
        private ZoomMenuItem _zoomMenuItem06;
        private ZoomMenuItem _zoomMenuItem07;
        private ZoomMenuItem _zoomMenuItem08;
        private ZoomMenuItem _zoomMenuItem09;
        private ZoomMenuItem _zoomMenuItem10;
        private ZoomMenuItem _zoomMenuItem11;
        private ZoomMenuItem _zoomMenuItem12;
        private ZoomMenuItem _zoomMenuItem13;
        private ZoomMenuItem _zoomMenuItem14;
        private ZoomMenuItem _zoomMenuItem15;

        private System.Windows.Forms.ToolTip filePathToolTip;
    }
}
