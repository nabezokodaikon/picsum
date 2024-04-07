using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{
    [SupportedOSPlatform("windows")]
    internal class FileInfoLabel
        : Control
    {
        private const float MARGIN = 8;

        private Color textColor = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);
        private SolidBrush textBrush = null;

        private string fileName = string.Empty;
        private string timestamp = string.Empty;
        private string fileType = string.Empty;
        private string fileSize = string.Empty;

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
            }
        }

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
            }
        }

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
            }
        }

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
            }
        }

        private SolidBrush TextBrush
        {
            get
            {
                if (this.textBrush == null)
                {
                    this.textBrush = new SolidBrush(this.textColor);
                }

                return this.textBrush;
            }
        }

        public FileInfoLabel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
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
                if (this.textBrush != null)
                {
                    this.textBrush.Dispose();
                    this.textBrush = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var textSize = e.Graphics.MeasureString("あ", this.Font);

            var fileNameRect = new RectangleF(
                MARGIN,
                MARGIN,
                this.Width - MARGIN * 2,
                textSize.Height * 2);

            if (!string.IsNullOrEmpty(this.FileName))
            {
                e.Graphics.DrawString(
                    this.FileName, this.Font, this.TextBrush, fileNameRect);

            }

            if (!string.IsNullOrEmpty(this.FileType))
            {
                e.Graphics.DrawString(
                    this.FileType, this.Font, this.TextBrush, MARGIN,
                    fileNameRect.Bottom + MARGIN);
            }

            if (!string.IsNullOrEmpty(this.Timestamp))
            {
                e.Graphics.DrawString(
                    this.Timestamp, this.Font, this.TextBrush, MARGIN,
                    fileNameRect.Bottom + +MARGIN + textSize.Height + MARGIN);
            }

            if (!string.IsNullOrEmpty(this.fileSize))
            {
                e.Graphics.DrawString(
                    this.fileSize, this.Font, this.TextBrush, MARGIN,
                    fileNameRect.Bottom + +MARGIN + (textSize.Height + MARGIN) * 2);
            }

            base.OnPaint(e);
        }
    }
}
