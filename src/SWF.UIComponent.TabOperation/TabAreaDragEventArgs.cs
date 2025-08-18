using System;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{

    public sealed class TabAreaDragEventArgs
        : DragEventArgs
    {
        public bool IsOverlap { get; private set; }
        public int TabIndex { get; private set; }

        public TabAreaDragEventArgs(bool isOverlap, int tabIndex, DragEventArgs e)
            : base(
                (e ?? throw new ArgumentNullException(nameof(e))).Data,
                e.KeyState,
                e.X,
                e.Y,
                e.AllowedEffect,
                e.Effect)
        {
            this.IsOverlap = isOverlap;
            this.TabIndex = tabIndex;
        }

    }
}
