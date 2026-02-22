using SWF.Core.Base;
using SWF.UIComponent.Base;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{

    public partial class FileListToolBar : ToolPanel
    {
        private static readonly Rectangle VIEW_BUTTON_DEFAULT_BOUNDS
            = new(3, 1, 64, 23);
        private static readonly Rectangle NAME_SORT_BUTTON_DEFAULT_BOUNDS
            = new(73, 1, 96, 23);
        private static readonly Rectangle THUMBNAIL_SIZE_SLIDER_DEFAULT_BOUNDS
            = new(481, 1, 108, 23);
        private static readonly Rectangle MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS
            = new(595, 1, 64, 23);

        public event EventHandler NameSortButtonClick;
        public event EventHandler PathSortButtonClick;
        public event EventHandler CreateDateSortButtonClick;
        public event EventHandler UpdateDateSortButtonClick;
        public event EventHandler TakenDateSortButtonClick;
        public event EventHandler AddDateSortButtonClick;
        public event EventHandler DirectoryMenuItemClick;
        public event EventHandler ImageFileMenuItemClick;
        public event EventHandler OtherFileMenuItemClick;
        public event EventHandler FileNameMenuItemClick;
        public event EventHandler ThumbnailSizeSliderBeginValueChange;
        public event EventHandler ThumbnailSizeSliderValueChanged;
        public event EventHandler ThumbnailSizeSliderValueChanging;
        public event EventHandler MovePreviewButtonClick;
        public event EventHandler MoveNextButtonClick;

        private bool _disposed = false;
        private bool _isShowingViewButtonDropDown = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DirectoryMenuItemChecked
        {
            get
            {
                return this._directoryMenuItem.Checked;
            }
            set
            {
                this._directoryMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ImageFileMenuItemChecked
        {
            get
            {
                return this._imageFileMenuItem.Checked;
            }
            set
            {
                this._imageFileMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OtherFileMenuItemChecked
        {
            get
            {
                return this._otherFileMenuItem.Checked;
            }
            set
            {
                this._otherFileMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FileNameMenuItemChecked
        {
            get
            {
                return this._fileNameMenuItem.Checked;
            }
            set
            {
                this._fileNameMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool NameSortButtonEnabled
        {
            get
            {
                return this._nameSortButton.Enabled;
            }
            set
            {
                this._nameSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PathSortButtonEnabled
        {
            get
            {
                return this._pathSortButton.Enabled;
            }
            set
            {
                this._pathSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UpdateDateSortButtonEnabled
        {
            get
            {
                return this._updateDateSortButton.Enabled;
            }
            set
            {
                this._updateDateSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TakenDateSortButtonEnabled
        {
            get
            {
                return this._takenDateSortButton.Enabled;
            }
            set
            {
                this._takenDateSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AddDateSortButtonEnabled
        {
            get
            {
                return this._addDateSortButton.Enabled;
            }
            set
            {
                this._addDateSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ThumbnailSizeSliderValue
        {
            get
            {
                return this._thumbnailSizeSlider.Value;
            }
            set
            {
                this._thumbnailSizeSlider.Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ThumbnailSizeSliderMaximumValue
        {
            get
            {
                return this._thumbnailSizeSlider.MaximumValue;
            }
            set
            {
                this._thumbnailSizeSlider.MaximumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ThumbnailSizeSliderMinimumValue
        {
            get
            {
                return this._thumbnailSizeSlider.MinimumValue;
            }
            set
            {
                this._thumbnailSizeSlider.MinimumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MovePreviewButtonVisible
        {
            get
            {
                return this._movePreviewButton.Visible;
            }
            set
            {
                this._movePreviewButton.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MoveNextButtonVisible
        {
            get
            {
                return this._moveNextButton.Visible;
            }
            set
            {
                this._moveNextButton.Visible = value;
            }
        }

        public FileListToolBar()
        {
            this.InitializeComponent();

            this._viewButton.LostFocus += this.ViewButton_LostFocus;
        }

        public void SetControlsBounds(float scale)
        {
            this._viewButton.SetBounds(
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.X * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Y * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(VIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._nameSortButton.SetBounds(
                this._viewButton.Location.X * 2 + this._viewButton.Width,
                this._viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._pathSortButton.SetBounds(
                this._viewButton.Location.X * 3 + this._viewButton.Width + this._nameSortButton.Width,
                this._viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._createDateSortButton.SetBounds(
                this._viewButton.Location.X * 4 + this._viewButton.Width + this._nameSortButton.Width * 2,
                this._viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._updateDateSortButton.SetBounds(
                this._viewButton.Location.X * 5 + this._viewButton.Width + this._nameSortButton.Width * 3,
                this._viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._takenDateSortButton.SetBounds(
                this._viewButton.Location.X * 6 + this._viewButton.Width + this._nameSortButton.Width * 4,
                this._viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._addDateSortButton.SetBounds(
                this._viewButton.Location.X * 7 + this._viewButton.Width + this._nameSortButton.Width * 5,
                this._viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._thumbnailSizeSlider.SetBounds(
                this._viewButton.Location.X * 8 + this._viewButton.Width + this._nameSortButton.Width * 6,
                this._viewButton.Location.Y,
                (int)(THUMBNAIL_SIZE_SLIDER_DEFAULT_BOUNDS.Width * scale),
                (int)(THUMBNAIL_SIZE_SLIDER_DEFAULT_BOUNDS.Height * scale));

            this._movePreviewButton.SetBounds(
                this._viewButton.Location.X * 9 + this._viewButton.Width + this._nameSortButton.Width * 6 + this._thumbnailSizeSlider.Width,
                this._viewButton.Location.Y,
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this._moveNextButton.SetBounds(
                this._viewButton.Location.X * 10 + this._viewButton.Width + this._nameSortButton.Width * 6 + this._thumbnailSizeSlider.Width + this._movePreviewButton.Width,
                this._viewButton.Location.Y,
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));
        }

        public BaseTextButton GetSortToolStripButton(FileSortMode sortMode)
        {
            return sortMode switch
            {
                FileSortMode.FileName => this._nameSortButton,
                FileSortMode.FilePath => this._pathSortButton,
                FileSortMode.CreateDate => this._createDateSortButton,
                FileSortMode.UpdateDate => this._updateDateSortButton,
                FileSortMode.TakenDate => this._takenDateSortButton,
                FileSortMode.AddDate => this._addDateSortButton,
                _ => null,
            };
        }

        public void ClearSortImage()
        {
            this._nameSortButton.Text = "Name";
            this._pathSortButton.Text = "Path";
            this._createDateSortButton.Text = "Created";
            this._updateDateSortButton.Text = "Updated";
            this._takenDateSortButton.Text = "Taken";
            this._addDateSortButton.Text = "Added";
        }

        protected override void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._viewButton.Dispose();
                this._thumbnailSizeSlider.Dispose();
                this._viewMenu.Dispose();
                this._directoryMenuItem.Dispose();
                this._imageFileMenuItem.Dispose();
                this._otherFileMenuItem.Dispose();
                this._toolStripSeparator1.Dispose();
                this._fileNameMenuItem.Dispose();
                this._movePreviewButton.Dispose();
                this._moveNextButton.Dispose();
                this._nameSortButton.Dispose();
                this._pathSortButton.Dispose();
                this._createDateSortButton.Dispose();
                this._updateDateSortButton.Dispose();
                this._takenDateSortButton.Dispose();
                this._addDateSortButton.Dispose();
            }

            this._disposed = true;

            base.Dispose(disposing);
        }

        private void ViewButton_LostFocus(object sender, EventArgs e)
        {
            this._isShowingViewButtonDropDown = false;
        }

        private void ViewButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (this._isShowingViewButtonDropDown)
            {
                this._isShowingViewButtonDropDown = false;
                this._viewMenu.Close();
            }
            else
            {
                this._isShowingViewButtonDropDown = true;
                this._viewMenu.Show(
                    this, new Point(this._viewButton.Left, this._viewButton.Bottom));
            }
        }

        private void NameSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.NameSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void PathSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.PathSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void CreateDateSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.CreateDateSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateDateSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.UpdateDateSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void TakenDateSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.TakenDateSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void AddDateSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.AddDateSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void DirectoryMenuItem_Click(object sender, EventArgs e)
        {
            this._isShowingViewButtonDropDown = false;
            this.DirectoryMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void ImageFileMenuItem_Click(object sender, EventArgs e)
        {
            this._isShowingViewButtonDropDown = false;
            this.ImageFileMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void OtherFileMenuItem_Click(object sender, EventArgs e)
        {
            this._isShowingViewButtonDropDown = false;
            this.OtherFileMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void FileNameMenuItem_Click(object sender, EventArgs e)
        {
            this._isShowingViewButtonDropDown = false;
            this.FileNameMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void ThumbnailSizeSlider_BeginValueChange(object sender, EventArgs e)
        {
            this.ThumbnailSizeSliderBeginValueChange?.Invoke(this, EventArgs.Empty);
        }

        private void ThumbnailSizeSlider_ValueChanged(object sender, EventArgs e)
        {
            this.ThumbnailSizeSliderValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ThumbnailSizeSlider_ValueChanging(object sender, EventArgs e)
        {
            this.ThumbnailSizeSliderValueChanging?.Invoke(this, EventArgs.Empty);
        }

        private void MovePreviewButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.MovePreviewButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void MoveNextButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.MoveNextButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
