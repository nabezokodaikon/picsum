using SWF.Core.Base;
using SWF.UIComponent.Core;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class FileListToolBar : ToolPanel
    {
        private static readonly Font DEFAULT_FONT = new Font("Yu Gothic UI", 12F);

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
        public event EventHandler TimestampSortButtonClick;
        public event EventHandler RegistrationSortButtonClick;
        public event EventHandler FolderMenuItemClick;
        public event EventHandler ImageFileMenuItemClick;
        public event EventHandler OtherFileMenuItemClick;
        public event EventHandler FileNameMenuItemClick;
        public event EventHandler ThumbnailSizeSliderBeginValueChange;
        public event EventHandler ThumbnailSizeSliderValueChanged;
        public event EventHandler ThumbnailSizeSliderValueChanging;
        public event EventHandler MovePreviewButtonClick;
        public event EventHandler MoveNextButtonClick;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FolderMenuItemChecked
        {
            get
            {
                return this.folderMenuItem.Checked;
            }
            set
            {
                this.folderMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ImageFileMenuItemChecked
        {
            get
            {
                return this.imageFileMenuItem.Checked;
            }
            set
            {
                this.imageFileMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool OtherFileMenuItemChecked
        {
            get
            {
                return this.otherFileMenuItem.Checked;
            }
            set
            {
                this.otherFileMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FileNameMenuItemChecked
        {
            get
            {
                return this.fileNameMenuItem.Checked;
            }
            set
            {
                this.fileNameMenuItem.Checked = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool NameSortButtonEnabled
        {
            get
            {
                return this.nameSortButton.Enabled;
            }
            set
            {
                this.nameSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PathSortButtonEnabled
        {
            get
            {
                return this.pathSortButton.Enabled;
            }
            set
            {
                this.pathSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TimestampSortButtonEnabled
        {
            get
            {
                return this.timestampSortButton.Enabled;
            }
            set
            {
                this.timestampSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RegistrationSortButtonEnabled
        {
            get
            {
                return this.registrationSortButton.Enabled;
            }
            set
            {
                this.registrationSortButton.Enabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ThumbnailSizeSliderValue
        {
            get
            {
                return this.thumbnailSizeSlider.Value;
            }
            set
            {
                this.thumbnailSizeSlider.Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ThumbnailSizeSliderMaximumValue
        {
            get
            {
                return this.thumbnailSizeSlider.MaximumValue;
            }
            set
            {
                this.thumbnailSizeSlider.MaximumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ThumbnailSizeSliderMinimumValue
        {
            get
            {
                return this.thumbnailSizeSlider.MinimumValue;
            }
            set
            {
                this.thumbnailSizeSlider.MinimumValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MovePreviewButtonVisible
        {
            get
            {
                return this.movePreviewButton.Visible;
            }
            set
            {
                this.movePreviewButton.Visible = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MoveNextButtonVisible
        {
            get
            {
                return this.moveNextButton.Visible;
            }
            set
            {
                this.moveNextButton.Visible = value;
            }
        }

        public FileListToolBar()
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

            this.nameSortButton.SetBounds(
                this.viewButton.Location.X * 2 + this.viewButton.Width,
                this.viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.pathSortButton.SetBounds(
                this.viewButton.Location.X * 3 + this.viewButton.Width + this.nameSortButton.Width,
                this.viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.timestampSortButton.SetBounds(
                this.viewButton.Location.X * 4 + this.viewButton.Width + this.nameSortButton.Width * 2,
                this.viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.registrationSortButton.SetBounds(
                this.viewButton.Location.X * 5 + this.viewButton.Width + this.nameSortButton.Width * 3,
                this.viewButton.Location.Y,
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(NAME_SORT_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.thumbnailSizeSlider.SetBounds(
                this.viewButton.Location.X * 6 + this.viewButton.Width + this.nameSortButton.Width * 4,
                this.viewButton.Location.Y,
                (int)(THUMBNAIL_SIZE_SLIDER_DEFAULT_BOUNDS.Width * scale),
                (int)(THUMBNAIL_SIZE_SLIDER_DEFAULT_BOUNDS.Height * scale));

            this.movePreviewButton.SetBounds(
                this.viewButton.Location.X * 7 + this.viewButton.Width + this.nameSortButton.Width * 4 + this.thumbnailSizeSlider.Width,
                this.viewButton.Location.Y,
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            this.moveNextButton.SetBounds(
                this.viewButton.Location.X * 8 + this.viewButton.Width + this.nameSortButton.Width * 4 + this.thumbnailSizeSlider.Width + this.movePreviewButton.Width,
                this.viewButton.Location.Y,
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Width * scale),
                (int)(MOVE_PREVIEW_BUTTON_DEFAULT_BOUNDS.Height * scale));

            if (this.viewButton.Font != null)
            {
                this.viewButton.Font.Dispose();
                this.viewButton.Font = null;
            }

            this.viewButton.Font = new Font(
                DEFAULT_FONT.FontFamily, DEFAULT_FONT.Size * scale, GraphicsUnit.Pixel);
            this.nameSortButton.Font = this.viewButton.Font;
            this.pathSortButton.Font = this.viewButton.Font;
            this.timestampSortButton.Font = this.viewButton.Font;
            this.registrationSortButton.Font = this.viewButton.Font;
            this.movePreviewButton.Font = this.viewButton.Font;
            this.moveNextButton.Font = this.viewButton.Font;

            this.viewButton.Text = "View";
            this.nameSortButton.Text = "Name";
            this.pathSortButton.Text = "Path";
            this.timestampSortButton.Text = "Time stamp";
            this.registrationSortButton.Text = "Registration";
            this.movePreviewButton.Text = "<-";
            this.moveNextButton.Text = "->";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public ToolTextButton GetSortToolStripButton(SortTypeID sortType)
        {
            return sortType switch
            {
                SortTypeID.FileName => this.nameSortButton,
                SortTypeID.FilePath => this.pathSortButton,
                SortTypeID.UpdateDate => this.timestampSortButton,
                SortTypeID.RegistrationDate => this.registrationSortButton,
                _ => null,
            };
        }

        public void ClearSortImage()
        {
            this.nameSortButton.Image = null;
            this.pathSortButton.Image = null;
            this.timestampSortButton.Image = null;
            this.registrationSortButton.Image = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Font = new Font(this.Font.FontFamily, 9F);
        }

        private void ViewButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.viewMenu.Show(
                this, new Point(this.viewButton.Left, this.viewButton.Bottom));
        }

        private void NameSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.NameSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void PathSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.PathSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void TimestampSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.TimestampSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void RegistrationSortButton_MouseClick(object sender, MouseEventArgs e)
        {
            this.RegistrationSortButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void FolderMenuItem_Click(object sender, EventArgs e)
        {
            this.FolderMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void ImageFileMenuItem_Click(object sender, EventArgs e)
        {
            this.ImageFileMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void OtherFileMenuItem_Click(object sender, EventArgs e)
        {
            this.OtherFileMenuItemClick?.Invoke(this, EventArgs.Empty);
        }

        private void FileNameMenuItem_Click(object sender, EventArgs e)
        {
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
