using SWF.UIComponent.Core;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal partial class InfoLabel
        : BasePaintingControl
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

        private string _fileName = string.Empty;
        private string _timestamp = string.Empty;
        private string _fileType = string.Empty;
        private string _fileSize = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName
        {
            get
            {
                return this._fileName;
            }
            set
            {
                this._fileName = value;
                this.Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Timestamp
        {
            get
            {
                return this._timestamp;
            }
            set
            {
                this._timestamp = value;
                this.Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileType
        {
            get
            {
                return this._fileType;
            }
            set
            {
                this._fileType = value;
                this.Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileSize
        {
            get
            {
                return this._fileSize;
            }
            set
            {
                this._fileSize = value;
                this.Invalidate();
            }
        }

        private SolidBrush TextBrush
        {
            get
            {
                return TEXT_BRUSH;
            }
        }

        public InfoLabel()
        {
            this.Paint += this.InfoLabel_Paint;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }

        private void InfoLabel_Paint(object sender, PaintEventArgs e)
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

            if (!string.IsNullOrEmpty(this._fileSize))
            {
                e.Graphics.DrawString(
                    this._fileSize, this.Font, this.TextBrush, 0,
                    fileNameRect.Bottom + MARGIN + (textSize.Height + MARGIN) * 2);
            }
        }
    }
}
