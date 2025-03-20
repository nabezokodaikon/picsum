using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.Contents.FileList
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class FileListToolBar : UserControl
    {
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
        public Image NameSortButtonImage
        {
            get
            {
                return this.nameSortButton.Image;
            }
            set
            {
                this.nameSortButton.Image = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image PathSortButtonImage
        {
            get
            {
                return this.pathSortButton.Image;
            }
            set
            {
                this.pathSortButton.Image = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image TimestampSortButtonImage
        {
            get
            {
                return this.timestampSortButton.Image;
            }
            set
            {
                this.timestampSortButton.Image = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image RegistrationDateSortButtonImage
        {
            get
            {
                return this.registrationSortButton.Image;
            }
            set
            {
                this.registrationSortButton.Image = value;
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
