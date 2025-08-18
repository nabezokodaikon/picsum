using System.ComponentModel;

namespace SWF.UIComponent.Base
{

    public class ToolPanel
        : BasePaintingControl
    {
        private const float BORDER_LINE_WIDTH = 1f;

        private static readonly SolidBrush BORDER_LINE_BRUSH =
            new SolidBrush(Color.FromArgb(200, 200, 200));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDrawLeftBorderLine { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDrawTopBorderLine { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDrawRightBorderLine { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDrawBottomBorderLine { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float VerticalTopMargin { get; set; } = 0f;

        public ToolPanel()
        {
            this.Paint += this.ToolPanel_Paint;
        }

        private void ToolPanel_Paint(object? sender, PaintEventArgs e)
        {
            if (this.IsDrawLeftBorderLine)
            {
                e.Graphics.FillRectangle(BORDER_LINE_BRUSH,
                    0, this.VerticalTopMargin, BORDER_LINE_WIDTH,
                    this.Height - this.VerticalTopMargin);
            }

            if (this.IsDrawTopBorderLine)
            {
                e.Graphics.FillRectangle(BORDER_LINE_BRUSH,
                    0, 0, this.Width, BORDER_LINE_WIDTH);
            }

            if (this.IsDrawRightBorderLine)
            {
                e.Graphics.FillRectangle(BORDER_LINE_BRUSH,
                    this.Width - BORDER_LINE_WIDTH, this.VerticalTopMargin,
                    BORDER_LINE_WIDTH, this.Height - this.VerticalTopMargin);
            }

            if (this.IsDrawBottomBorderLine)
            {
                e.Graphics.FillRectangle(BORDER_LINE_BRUSH,
                    0, this.Height - BORDER_LINE_WIDTH, this.Width, BORDER_LINE_WIDTH);
            }
        }
    }
}
