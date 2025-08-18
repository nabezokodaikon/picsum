using SWF.Core.Base;

namespace SWF.UIComponent.Base
{

    public class BaseIconButton
        : BaseButton
    {
        public BaseIconButton()
        {

        }

        protected override void Draw(PaintEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            if (this.Image == null)
            {
                return;
            }

            var scale = WindowUtil.GetCurrentWindowScale(this);
            var length = Math.Min(this.Width - 12 * scale, this.Height - 12 * scale);
            var w = length;
            var h = length;
            var x = (this.Width - w) / 2f;
            var y = (this.Height - h) / 2f;

            e.Graphics.DrawImage(this.Image, x, y, w, h);
        }
    }
}
