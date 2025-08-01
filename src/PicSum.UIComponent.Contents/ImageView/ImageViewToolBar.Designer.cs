using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Core;
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
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip();
            this.singleViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadLeftFeedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadRightFeedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doublePreviewButton = new SWF.UIComponent.Core.BaseTextButton();
            this.singlePreviewButton = new SWF.UIComponent.Core.BaseTextButton();
            this.doubleNextButton = new SWF.UIComponent.Core.BaseTextButton();
            this.singleNextButton = new SWF.UIComponent.Core.BaseTextButton();
            this.indexSlider = new SWF.UIComponent.Core.Slider();
            this.viewButton = new SWF.UIComponent.Core.BaseTextButton();
            this.sizeButton = new SWF.UIComponent.Core.BaseTextButton();
            this.sizeMenu = new System.Windows.Forms.ContextMenuStrip();
            this.originalSizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitWindowLargeOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.sizeMenuSeparator = new ToolStripSeparator();
            this.zoomMenuItem01 = new ZoomMenuItem(0.1f, "10%");
            this.zoomMenuItem02 = new ZoomMenuItem(0.25f, "25%");
            this.zoomMenuItem03 = new ZoomMenuItem(0.33f, "33%");
            this.zoomMenuItem04 = new ZoomMenuItem(0.5f, "50%");
            this.zoomMenuItem05 = new ZoomMenuItem(0.66f, "66%");
            this.zoomMenuItem06 = new ZoomMenuItem(0.75f, "75%");
            this.zoomMenuItem07 = new ZoomMenuItem(AppConstants.DEFAULT_ZOOM_VALUE, "100%");
            this.zoomMenuItem08 = new ZoomMenuItem(1.25f, "125%");
            this.zoomMenuItem09 = new ZoomMenuItem(1.5f, "150%");
            this.zoomMenuItem10 = new ZoomMenuItem(2f, "200%");
            this.zoomMenuItem11 = new ZoomMenuItem(3f, "300%");
            this.zoomMenuItem12 = new ZoomMenuItem(5f, "500%");
            this.zoomMenuItem13 = new ZoomMenuItem(10f, "1000%");
            this.zoomMenuItem14 = new ZoomMenuItem(20f, "2000%");
            this.zoomMenuItem15 = new ZoomMenuItem(30f, "3000%");

            this.filePathToolTip = new System.Windows.Forms.ToolTip();
            this.viewMenu.SuspendLayout();
            this.sizeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewMenu
            //
            this.viewMenu.Font = Fonts.GetRegularFont(Fonts.Size.Medium);
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.singleViewMenuItem, this.spreadLeftFeedMenuItem, this.spreadRightFeedMenuItem });
            // 
            // singleViewMenuItem
            // 
            this.singleViewMenuItem.Text = "Single View";
            this.singleViewMenuItem.Click += this.SingleViewMenuItem_Click;
            // 
            // spreadLeftFeedMenuItem
            // 
            this.spreadLeftFeedMenuItem.Text = "Spread (Left Feed)";
            this.spreadLeftFeedMenuItem.Click += this.SpreadLeftFeedMenuItem_Click;
            // 
            // spreadRightFeedMenuItem
            // 
            this.spreadRightFeedMenuItem.Text = "Spread (Right Feed)";
            this.spreadRightFeedMenuItem.Click += this.SpreadRightFeedMenuItem_Click;
            // 
            // doublePreviewButton
            //
            this.doublePreviewButton.Text = "<<-";
            this.doublePreviewButton.MouseClick += this.DoublePreviewButton_MouseClick;
            // 
            // singlePreviewButton
            //
            this.singlePreviewButton.Text = "<-";
            this.singlePreviewButton.MouseClick += this.SinglePreviewButton_MouseClick;
            // 
            // doubleNextButton
            //
            this.doubleNextButton.Text = "->>";
            this.doubleNextButton.MouseClick += this.DoubleNextButton_MouseClick;
            // 
            // singleNextButton
            //
            this.singleNextButton.Text = "->";
            this.singleNextButton.MouseClick += this.SingleNextButton_MouseClick;
            // 
            // indexSlider
            // 
            this.indexSlider.BeginValueChange += this.IndexSlider_BeginValueChange;
            this.indexSlider.ValueChanging += this.IndexSlider_ValueChanging;
            this.indexSlider.ValueChanged += this.IndexSlider_ValueChanged;
            this.indexSlider.MouseUp += this.IndexSlider_MouseUp;
            // 
            // viewButton
            //
            this.viewButton.Text = "View";
            this.viewButton.MouseClick += this.ViewButton_MouseClick;
            // 
            // sizeButton
            //
            this.sizeButton.Text = "Size";
            this.sizeButton.MouseClick += this.SizeButton_MouseClick;
            // 
            // sizeMenu
            //
            this.sizeMenu.Font = Fonts.GetRegularFont(Fonts.Size.Medium);
            this.sizeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.fitWindowMenuItem,
                this.fitWindowLargeOnlyMenuItem,
                this.sizeMenuSeparator,
                this.zoomMenuItem01,
                this.zoomMenuItem02,
                this.zoomMenuItem03,
                this.zoomMenuItem04,
                this.zoomMenuItem05,
                this.zoomMenuItem06,
                //this.zoomMenuItem07,
                this.originalSizeMenuItem,
                this.zoomMenuItem08,
                this.zoomMenuItem09,
                this.zoomMenuItem10,
                this.zoomMenuItem11,
                this.zoomMenuItem12,
                this.zoomMenuItem13,
                this.zoomMenuItem14,
                this.zoomMenuItem15,
            });
            // 
            // originalSizeMenuItem
            // 
            this.originalSizeMenuItem.Text = "100%";
            this.originalSizeMenuItem.Click += this.OriginalSizeMenuItem_Click;
            // 
            // fitWindowMenuItem
            // 
            this.fitWindowMenuItem.Text = "Fit To Window";
            this.fitWindowMenuItem.Click += this.FitWindowMenuItem_Click;
            // 
            // fitWindowLargeOnlyMenuItem
            // 
            this.fitWindowLargeOnlyMenuItem.Text = "Fit To Window (Large Image Only)";
            this.fitWindowLargeOnlyMenuItem.Click += this.FitWindowLargeOnlyMenuItem_Click;
            // 
            // zoomMenuItem
            // 
            this.zoomMenuItem01.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem02.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem03.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem04.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem05.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem06.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem07.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem08.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem09.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem10.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem11.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem12.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem13.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem14.Click += this.ZoomMenuItem_Click;
            this.zoomMenuItem15.Click += this.ZoomMenuItem_Click;
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
                this.sizeButton,
                this.viewButton,
                this.indexSlider,
                this.singleNextButton,
                this.doubleNextButton,
                this.singlePreviewButton,
                this.doublePreviewButton);
            this.viewMenu.ResumeLayout(false);
            this.sizeMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ToolStripDropDown viewMenu;
        private SWF.UIComponent.Core.BaseTextButton doublePreviewButton;
        private SWF.UIComponent.Core.BaseTextButton singlePreviewButton;
        private SWF.UIComponent.Core.BaseTextButton doubleNextButton;
        private SWF.UIComponent.Core.BaseTextButton singleNextButton;
        private SWF.UIComponent.Core.Slider indexSlider;
        private SWF.UIComponent.Core.BaseTextButton viewButton;
        private System.Windows.Forms.ToolStripMenuItem singleViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadLeftFeedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadRightFeedMenuItem;
        private SWF.UIComponent.Core.BaseTextButton sizeButton;
        private System.Windows.Forms.ToolStripDropDown sizeMenu;
        private System.Windows.Forms.ToolStripMenuItem originalSizeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitWindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitWindowLargeOnlyMenuItem;

        private System.Windows.Forms.ToolStripSeparator sizeMenuSeparator;
        private ZoomMenuItem zoomMenuItem01;
        private ZoomMenuItem zoomMenuItem02;
        private ZoomMenuItem zoomMenuItem03;
        private ZoomMenuItem zoomMenuItem04;
        private ZoomMenuItem zoomMenuItem05;
        private ZoomMenuItem zoomMenuItem06;
        private ZoomMenuItem zoomMenuItem07;
        private ZoomMenuItem zoomMenuItem08;
        private ZoomMenuItem zoomMenuItem09;
        private ZoomMenuItem zoomMenuItem10;
        private ZoomMenuItem zoomMenuItem11;
        private ZoomMenuItem zoomMenuItem12;
        private ZoomMenuItem zoomMenuItem13;
        private ZoomMenuItem zoomMenuItem14;
        private ZoomMenuItem zoomMenuItem15;

        private System.Windows.Forms.ToolTip filePathToolTip;
    }
}
