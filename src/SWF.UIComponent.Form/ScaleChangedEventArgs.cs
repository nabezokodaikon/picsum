using System;

namespace SWF.UIComponent.Form
{
    public sealed class ScaleChangedEventArgs
        : EventArgs
    {
        public float Scale { get; private set; }

        public ScaleChangedEventArgs(float scale)
        {
            this.Scale = scale;
        }
    }
}
