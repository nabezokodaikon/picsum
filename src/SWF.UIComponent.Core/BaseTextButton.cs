using SWF.Core.Base;
using SWF.Core.ResourceAccessor;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseTextButton
        : BaseButton
    {
        private static readonly Color TEXT_COLOR = Color.FromArgb(
             SystemColors.ControlText.A,
             SystemColors.ControlText.R,
             SystemColors.ControlText.G,
             SystemColors.ControlText.B);

        public BaseTextButton()
        {

        }

        protected override void Draw(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                return;
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var font = Fonts.GetRegularFont(Fonts.Size.Small, scale);
            var textSize = TextRenderer.MeasureText(this.Text, font);
            var textRect = new Rectangle(
                this.Width - textSize.Width - ((this.Width - textSize.Width) / 2),
                this.Height - textSize.Height - ((this.Height - textSize.Height) / 2),
                textSize.Width,
                textSize.Height
                );

            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                font,
                textRect.Location,
                TEXT_COLOR,
                TextFormatFlags.Left);
        }
    }
}
