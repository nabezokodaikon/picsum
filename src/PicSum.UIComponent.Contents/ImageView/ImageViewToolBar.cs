using SWF.Core.Base;
using SWF.UIComponent.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.ImageView
{

    public partial class ImageViewToolBar : ToolPanel
    {
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
        public event EventHandler<ZoomMenuItemClickEventArgs> ZoomMenuItemClick;

        public event EventHandler IndexSliderBeginValueChange;
        public event EventHandler IndexSliderValueChanged;
        public event EventHandler IndexSliderValueChanging;

        private bool _disposed = false;
        private bool _isShowingMenuButtonDropDown = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndexSliderValue
        {
            get
            {
                return this._indexSlider.Value;
            }
            set
            {
                this._indexSlider.Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndexSliderMaximumValue
        {
            get
            {
                return this._indexSlider.MaximumValue;
            }
            set
            {
                this._indexSlider.MaximumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IndexSliderMinimumValue
        {
            get
            {
                return this._indexSlider.MinimumValue;
            }
            set
            {
                this._indexSlider.MinimumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SingleViewMenuItemChecked
        {
            get
            {
                return this._singleViewMenuItem.Checked;
            }
            set
            {
                this._singleViewMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SpreadLeftFeedMenuItemChecked
        {
            get
            {
                return this._spreadLeftFeedMenuItem.Checked;
            }
            set
            {
                this._spreadLeftFeedMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SpreadRightFeedMenuItemChecked
        {
            get
            {
                return this._spreadRightFeedMenuItem.Checked;
            }
            set
            {
                this._spreadRightFeedMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OriginalSizeMenuItemChecked
        {
            get
            {
                return this._originalSizeMenuItem.Checked;
            }
            set
            {
                this._originalSizeMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FitWindowMenuItemChecked
        {
            get
            {
                return this._fitWindowMenuItem.Checked;
            }
            set
            {
                this._fitWindowMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FitWindowLargeOnlyMenuItemChecked
        {
            get
            {
                return this._fitWindowLargeOnlyMenuItem.Checked;
            }
            set
            {
                this._fitWindowLargeOnlyMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DoubleNextButtonEnabled
        {
            get
            {
                return this._doubleNextButton.Enabled;
            }
            set
            {
                this._doubleNextButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DoublePreviewButtonEnabled
        {
            get
            {
                return this._doublePreviewButton.Enabled;
            }
            set
            {
                this._doublePreviewButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SingleNextButtonEnabled
        {
            get
            {
                return this._singleNextButton.Enabled;
            }
            set
            {
                this._singleNextButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SinglePreviewButtonEnabled
        {
            get
            {
                return this._singlePreviewButton.Enabled;
            }
            set
            {
                this._singlePreviewButton.Enabled = value;
            }
        }

        public ImageViewToolBar()
        {
            this.InitializeComponent();

            this._viewButton.LostFocus += this.MenuButton_LostFocus;
            this._sizeButton.LostFocus += this.MenuButton_LostFocus;
        }

        public void SetControlsBounds(float scale)
        {
            this._viewButton.SetBounds(
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.X * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Y * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._sizeButton.SetBounds(
                this._viewButton.Location.X * 2 + this._viewButton.Width,
                this._viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._doublePreviewButton.SetBounds(
                this._viewButton.Location.X * 3 + this._viewButton.Width * 2,
                this._viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._singlePreviewButton.SetBounds(
                this._viewButton.Location.X * 4 + this._viewButton.Width * 3,
                this._viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._singleNextButton.SetBounds(
                this._viewButton.Location.X * 5 + this._viewButton.Width * 4,
                this._viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._doubleNextButton.SetBounds(
                this._viewButton.Location.X * 6 + this._viewButton.Width * 5,
                this._viewButton.Location.Y,
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._indexSlider.SetBounds(
                this._viewButton.Location.X * 7 + this._viewButton.Width * 6,
                this._viewButton.Location.Y,
                (int)(this.Width - (this._viewButton.Location.X * 7 + this._viewButton.Width * 6 + 16 * scale)),
                (int)(INDEX_SLIDER_DEFAULT_BOUNDS.Height * scale));

            this._indexSlider.Anchor
                = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._viewMenu.Dispose();
                this._doublePreviewButton.Dispose();
                this._singlePreviewButton.Dispose();
                this._doubleNextButton.Dispose();
                this._singleNextButton.Dispose();
                this._indexSlider.Dispose();
                this._viewButton.Dispose();
                this._singleViewMenuItem.Dispose();
                this._spreadLeftFeedMenuItem.Dispose();
                this._spreadRightFeedMenuItem.Dispose();
                this._sizeButton.Dispose();
                this._sizeMenu.Dispose();
                this._originalSizeMenuItem.Dispose();
                this._fitWindowMenuItem.Dispose();
                this._fitWindowLargeOnlyMenuItem.Dispose();
                this._sizeMenuSeparator.Dispose();
                this.filePathToolTip.Dispose();
                this._zoomMenuItem01.Dispose();
                this._zoomMenuItem02.Dispose();
                this._zoomMenuItem03.Dispose();
                this._zoomMenuItem04.Dispose();
                this._zoomMenuItem05.Dispose();
                this._zoomMenuItem06.Dispose();
                this._zoomMenuItem07.Dispose();
                this._zoomMenuItem08.Dispose();
                this._zoomMenuItem09.Dispose();
                this._zoomMenuItem10.Dispose();
                this._zoomMenuItem11.Dispose();
                this._zoomMenuItem12.Dispose();
                this._zoomMenuItem13.Dispose();
                this._zoomMenuItem14.Dispose();
                this._zoomMenuItem15.Dispose();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        public void ShowToolTip(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            var p = this.PointToClient(Cursor.Position);
            this.filePathToolTip.Show(filePath, this, p.X, this.Height, 5000);
        }

        public ZoomMenuItem[] GetZoomMenuItems()
        {
            return [
                this._zoomMenuItem01,
                this._zoomMenuItem02,
                this._zoomMenuItem03,
                this._zoomMenuItem04,
                this._zoomMenuItem05,
                this._zoomMenuItem06,
                this._zoomMenuItem07,
                this._zoomMenuItem08,
                this._zoomMenuItem09,
                this._zoomMenuItem10,
                this._zoomMenuItem11,
                this._zoomMenuItem12,
                this._zoomMenuItem13,
                this._zoomMenuItem14,
                this._zoomMenuItem15,
            ];
        }

        public float GetZoomValue()
        {
            foreach (var item in this.GetZoomMenuItems())
            {
                if (item.Checked)
                {
                    return item.ZoomValue;
                }
            }

            return AppConstants.DEFAULT_ZOOM_VALUE;
        }

        private void MenuButton_LostFocus(object sender, EventArgs e)
        {
            this._isShowingMenuButtonDropDown = false;
        }

        private void ViewButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._isShowingMenuButtonDropDown)
            {
                this._isShowingMenuButtonDropDown = false;
                this._viewMenu.Close();
            }
            else
            {
                this._isShowingMenuButtonDropDown = true;
                this._viewMenu.Show(
                    this, new Point(this._viewButton.Left, this._viewButton.Bottom));
            }
        }

        private void SizeButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._isShowingMenuButtonDropDown)
            {
                this._isShowingMenuButtonDropDown = false;
                this._sizeMenu.Close();
            }
            else
            {
                this._isShowingMenuButtonDropDown = true;
                this._sizeMenu.Show(
                    this, new Point(this._sizeButton.Left, this._sizeButton.Bottom));
            }
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

        private void IndexSlider_MouseUp(object sender, MouseEventArgs e)
        {
            this.filePathToolTip.Hide(this);
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

        private void ZoomMenuItem_Click(object sender, ZoomMenuItemClickEventArgs e)
        {
            this.ZoomMenuItemClick?.Invoke(this, e);
        }
    }
}
