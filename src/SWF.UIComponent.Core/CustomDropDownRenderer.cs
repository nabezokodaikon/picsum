using SWF.Core.ResourceAccessor;
using System.Runtime.Versioning;

namespace SWF.UIComponent.Core
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class CustomDropDownRenderer
        : ToolStripProfessionalRenderer
    {
        private readonly ToolStripDropDown _owner;

        public CustomDropDownRenderer(ToolStripDropDown owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            this._owner = owner;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var scale = WindowUtil.GetCurrentWindowScale(this._owner);
            e.TextFont = Fonts.GetRegularFont(Fonts.Size.Medium, scale);

            base.OnRenderItemText(e);
        }
    }
}
