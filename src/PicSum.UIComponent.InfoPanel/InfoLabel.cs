using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal partial class FileInfoLabel
        : Control
    {
        private static readonly Color TEXT_COLOR = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);
        private static readonly SolidBrush TEXT_BRUSH = new SolidBrush(TEXT_COLOR);
        private static readonly StringFormat STRING_FORMAT = new()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near,
            Trimming = StringTrimming.EllipsisCharacter,
        };

        private string fileName = string.Empty;
        private string timestamp = string.Empty;
        private string fileType = string.Empty;
        private string fileSize = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
                this.Invalidate();
                this.Update();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Timestamp
        {
            get
            {
                return this.timestamp;
            }
            set
            {
                this.timestamp = value;
                this.Invalidate();
                this.Update();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileType
        {
            get
            {
                return this.fileType;
            }
            set
            {
                this.fileType = value;
                this.Invalidate();
                this.Update();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileSize
        {
            get
            {
                return this.fileSize;
            }
            set
            {
                this.fileSize = value;
                this.Invalidate();
                this.Update();
            }
        }

        private SolidBrush TextBrush
        {
            get
            {
                return TEXT_BRUSH;
            }
        }

        public FileInfoLabel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint,
                true);
            this.UpdateStyles();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            const float MARGIN = 8;
            var textSize = e.Graphics.MeasureString("あ", this.Font);

            var fileNameRect = new RectangleF(
                0,
                0,
                this.Width,
                textSize.Height * 2);

            if (!string.IsNullOrEmpty(this.FileName))
            {
                e.Graphics.DrawString(
                    this.FileName, this.Font, this.TextBrush, fileNameRect, STRING_FORMAT);
            }

            if (!string.IsNullOrEmpty(this.FileType))
            {
                e.Graphics.DrawString(
                    this.FileType, this.Font, this.TextBrush, 0,
                    fileNameRect.Bottom + MARGIN);
            }

            if (!string.IsNullOrEmpty(this.Timestamp))
            {
                e.Graphics.DrawString(
                    this.Timestamp, this.Font, this.TextBrush, 0,
                    fileNameRect.Bottom + MARGIN + textSize.Height + MARGIN);
            }

            if (!string.IsNullOrEmpty(this.fileSize))
            {
                e.Graphics.DrawString(
                    this.fileSize, this.Font, this.TextBrush, 0,
                    fileNameRect.Bottom + MARGIN + (textSize.Height + MARGIN) * 2);
            }

            base.OnPaint(e);
        }
    }
}
