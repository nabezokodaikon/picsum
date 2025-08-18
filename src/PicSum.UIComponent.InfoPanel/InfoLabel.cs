using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using SWF.UIComponent.Base;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PicSum.UIComponent.InfoPanel
{

    internal sealed partial class InfoLabel
        : BasePaintingControl
    {
        private static readonly Color TEXT_COLOR = Color.FromArgb(
            SystemColors.ControlText.A,
            SystemColors.ControlText.R,
            SystemColors.ControlText.G,
            SystemColors.ControlText.B);
        private static readonly SolidBrush TEXT_BRUSH = new(TEXT_COLOR);
        private static readonly StringFormat STRING_FORMAT = new()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near,
            Trimming = StringTrimming.EllipsisCharacter,
        };

        private string _fileName = string.Empty;
        private string _createDate = string.Empty;
        private string _updateDate = string.Empty;
        private string _takenDate = string.Empty;
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
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CreateDate
        {
            get
            {
                return this._createDate;
            }
            set
            {
                this._createDate = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UpdateDate
        {
            get
            {
                return this._updateDate;
            }
            set
            {
                this._updateDate = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TakenDate
        {
            get
            {
                return this._takenDate;
            }
            set
            {
                this._takenDate = value;
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
            var scale = WindowUtil.GetCurrentWindowScale(this);
            var margin = 8 * scale;
            var font = Fonts.GetRegularFont(Fonts.Size.Medium, scale);
            var textSize = e.Graphics.MeasureString("あ", font);

            var fileNameRect = new RectangleF(
                0,
                0,
                this.Width,
                textSize.Height * 2);

            if (!string.IsNullOrEmpty(this.FileName))
            {
                e.Graphics.DrawString(
                    this.FileName, font, this.TextBrush, fileNameRect, STRING_FORMAT);
            }

            if (!string.IsNullOrEmpty(this.FileType))
            {
                e.Graphics.DrawString(
                    this.FileType, font, this.TextBrush, 0,
                    fileNameRect.Bottom + margin);
            }

            if (!string.IsNullOrEmpty(this.FileSize))
            {
                e.Graphics.DrawString(
                    this._fileSize, font, this.TextBrush, 0,
                    fileNameRect.Bottom + margin + textSize.Height + margin);

            }

            var dateHeaderSize = e.Graphics.MeasureString("Updated", font);

            if (!string.IsNullOrEmpty(this.CreateDate))
            {
                e.Graphics.DrawString(
                    "Created", font, this.TextBrush, 0,
                    fileNameRect.Bottom + margin + (textSize.Height + margin) * 2);
                e.Graphics.DrawString(
                    $"{this.CreateDate}", font, this.TextBrush, dateHeaderSize.Width,
                    fileNameRect.Bottom + margin + (textSize.Height + margin) * 2);
            }

            if (!string.IsNullOrEmpty(this.UpdateDate))
            {
                e.Graphics.DrawString(
                    "Updated", font, this.TextBrush, 0,
                    fileNameRect.Bottom + margin + (textSize.Height + margin) * 3);
                e.Graphics.DrawString(
                    $"{this.UpdateDate}", font, this.TextBrush, dateHeaderSize.Width,
                    fileNameRect.Bottom + margin + (textSize.Height + margin) * 3);
            }

            if (!string.IsNullOrEmpty(this.TakenDate))
            {
                e.Graphics.DrawString(
                    "Taken", font, this.TextBrush, 0,
                    fileNameRect.Bottom + margin + (textSize.Height + margin) * 4);
                e.Graphics.DrawString(
                    $"{this.TakenDate}", font, this.TextBrush, dateHeaderSize.Width,
                    fileNameRect.Bottom + margin + (textSize.Height + margin) * 4);
            }
        }
    }
}
