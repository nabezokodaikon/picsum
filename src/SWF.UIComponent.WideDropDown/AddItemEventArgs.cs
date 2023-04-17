using System;

namespace SWF.UIComponent.WideDropDown
{
    public sealed class AddItemEventArgs
        : EventArgs
    {
        public string Item { get; private set; }

        public AddItemEventArgs(string item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.Item = item;
        }
    }
}
