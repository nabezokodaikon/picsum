using SWF.Core.Base;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public class BaseIconButton
        : BaseButton
    {
        public BaseIconButton()
        {

        }

        protected override void Draw(PaintEventArgs e)
        {
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
