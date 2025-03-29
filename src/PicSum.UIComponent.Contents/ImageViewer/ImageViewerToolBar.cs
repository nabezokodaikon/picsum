using SWF.UIComponent.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageViewer
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class ImageViewerToolBar : ToolPanel
    {
        private static readonly Font DEFAULT_FONT = new Font("Yu Gothic UI", 12F);

        private static readonly Rectangle VIEW_BUTTON_DEFAULT_BOUNDS
            = new(3, 1, 64, 23);
        private static readonly Rectangle INDEX_SLIDER_DEFAULT_BOUNDS
            = new(423, 1, 253, 23);

        public event EventHandler DoubleNextButtonClick;
        public event EventHandler DoublePreviewButtonClick;
        public event EventHandler SinglePreviewButtonClick;
        public event EventHandler SingleNextButtonClick;

        public event EventHandler SingleViewMenuItemClick;
        public event EventHandler SpreadLeftFeedMenuItemClick;
        public event EventHandler SpreadRightFeedMenuItemClick;
        public event EventHandler OriginalSizeMenuItemClick;
        public event EventHandler FitWindowMenuItemClick;
        public event EventHandler FitWindowLargeOnlyMenuItemClick;

        public event EventHandler IndexSliderBeginValueChange;
        public event EventHandler IndexSliderValueChanged;
        public event EventHandler IndexSliderValueChanging;
        public event EventHandler IndexSliderMouseLeave;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndexSliderValue
        {
            get
            {
                return this.indexSlider.Value;
            }
            set
            {
                this.indexSlider.Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndexSliderMaximumValue
        {
            get
            {
                return this.indexSlider.MaximumValue;
            }
            set
            {
                this.indexSlider.MaximumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndexSliderMinimumValue
        {
            get
            {
                return this.indexSlider.MinimumValue;
            }
            set
            {
                this.indexSlider.MinimumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SingleViewMenuItemChecked
        {
            get
            {
                return this.singleViewMenuItem.Checked;
            }
            set
            {
                this.singleViewMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SpreadLeftFeedMenuItemChecked
        {
            get
            {
                return this.spreadLeftFeedMenuItem.Checked;
            }
            set
            {
                this.spreadLeftFeedMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SpreadRightFeedMenuItemChecked
        {
            get
            {
                return this.spreadRightFeedMenuItem.Checked;
            }
            set
            {
                this.spreadRightFeedMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OriginalSizeMenuItemChecked
        {
            get
            {
                return this.originalSizeMenuItem.Checked;
            }
            set
            {
                this.originalSizeMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FitWindowMenuItemChecked
        {
            get
            {
                return this.fitWindowMenuItem.Checked;
            }
            set
            {
                this.fitWindowMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FitWindowLargeOnlyMenuItemChecked
        {
            get
            {
                return this.fitWindowLargeOnlyMenuItem.Checked;
            }
            set
            {
                this.fitWindowLargeOnlyMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DoubleNextButtonEnabled
        {
            get
            {
                return this.doubleNextButton.Enabled;
            }
            set
            {
                this.doubleNextButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DoublePreviewButtonEnabled
        {
            get
            {
                return this.doublePreviewButton.Enabled;
            }
            set
            {
                this.doublePreviewButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SingleNextButtonEnabled
        {
            get
            {
                return this.singleNextButton.Enabled;
            }
            set
            {
                this.singleNextButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SinglePreviewButtonEnabled
        {
            get
            {
                return this.singlePreviewButton.Enabled;
            }
            set
            {
                this.singlePreviewButton.Enabled = value;
            }
        }

        public ImageViewerToolBar()
        {
            this.InitializeComponent();
        }

        public void SetControlsBounds(float scale)
        {
            this.SuspendLayout();

            this.viewButton.SetBounds(
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.X * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Y * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.sizeButton.SetBounds(
                this.viewButton.Location.X * 2 + this.viewButton.Width,
                this.viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.doublePreviewButton.SetBounds(
                this.viewButton.Location.X * 3 + this.viewButton.Width * 2,
                this.viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.singlePreviewButton.SetBounds(
                this.viewButton.Location.X * 4 + this.viewButton.Width * 3,
                this.viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.singleNextButton.SetBounds(
                this.viewButton.Location.X * 5 + this.viewButton.Width * 4,
                this.viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.doubleNextButton.SetBounds(
                this.viewButton.Location.X * 6 + this.viewButton.Width * 5,
                this.viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.indexSlider.SetBounds(
                this.viewButton.Location.X * 7 + this.viewButton.Width * 6,
                this.viewButton.Location.Y,
                (int)(this.Width - (this.viewButton.Location.X * 7 + this.viewButton.Width * 6 + 16 * scale)),
                (int)(INDEX_SLIDER_DEFAULT_BOUNDS.Height * scale));

            this.indexSlider.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            if (this.viewButton.Font != null)
            {
                this.viewButton.Font.Dispose();
                this.viewButton.Font = null;
            }

            this.viewButton.Font = new Font(
                DEFAULT_FONT.FontFamily, DEFAULT_FONT.Size * scale, GraphicsUnit.Pixel);
            this.sizeButton.Font = this.viewButton.Font;
            this.doublePreviewButton.Font = this.viewButton.Font;
            this.singlePreviewButton.Font = this.viewButton.Font;
            this.singleNextButton.Font = this.viewButton.Font;
            this.doubleNextButton.Font = this.viewButton.Font;

            this.viewButton.Text = "View";
            this.sizeButton.Text = "Size";
            this.doublePreviewButton.Text = "<<-";
            this.singlePreviewButton.Text = "<-";
            this.singleNextButton.Text = "->";
            this.doubleNextButton.Text = "->>";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Font = new Font(this.Font.FontFamily, 9F);
        }

        public void ShowToolTip(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var p = this.PointToClient(Cursor.Position);
            this.filePathToolTip.Show(filePath, this, p.X, -16, 5000);
        }

        public void HideToolTip()
        {
            this.filePathToolTip.Hide(this);
        }

        private void ViewButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.viewMenu.Show(
                this, new Point(this.viewButton.Left, this.viewButton.Bottom));
        }

        private void SizeButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.sizeMenu.Show(
                this, new Point(this.sizeButton.Left, this.sizeButton.Bottom));
        }

        private void DoubleNextButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.DoubleNextButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void DoublePreviewButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.DoublePreviewButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void SinglePreviewButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.SinglePreviewButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void SingleNextButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.SingleNextButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void IndexSlider_BeginValueChange(object sender, EventArgs e)
        {
            this.IndexSliderBeginValueChange?.Invoke(this, EventArgs.Empty);
        }

        private void IndexSlider_ValueChanged(object sender, EventArgs e)
        {
            this.IndexSliderValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void IndexSlider_MouseLeave(object sender, System.EventArgs e)
        {
            this.IndexSliderMouseLeave?.Invoke(this, EventArgs.Empty);
        }

        private void IndexSlider_ValueChanging(object sender, EventArgs e)
        {
            this.IndexSliderValueChanging?.Invoke(this, EventArgs.Empty);
        }

        private void SingleViewMenuItem_Click(object sender, EventArgs e)
        {
            this.SingleViewMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void SpreadLeftFeedMenuItem_Click(object sender, EventArgs e)
        {
            this.SpreadLeftFeedMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void SpreadRightFeedMenuItem_Click(object sender, EventArgs e)
        {
            this.SpreadRightFeedMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void OriginalSizeMenuItem_Click(object sender, EventArgs e)
        {
            this.OriginalSizeMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void FitWindowMenuItem_Click(object sender, EventArgs e)
        {
            this.FitWindowMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void FitWindowLargeOnlyMenuItem_Click(object sender, EventArgs e)
        {
            this.FitWindowLargeOnlyMenuItemClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
