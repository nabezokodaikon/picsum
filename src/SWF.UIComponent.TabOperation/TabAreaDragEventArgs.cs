using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    [SupportedOSPlatform("windows")]
    public sealed class TabAreaDragEventArgs
        : DragEventArgs
    {
        public bool IsOverlap { get; private set; }
        public int TabIndex { get; private set; }

        public TabAreaDragEventArgs(bool isOverlap, int tabIndex, DragEventArgs e)
            : base(e.Data, e.KeyState, e.X, e.Y, e.AllowedEffect, e.Effect)
        {
            this.IsOverlap = isOverlap;
            this.TabIndex = tabIndex;
        }
    }
}
