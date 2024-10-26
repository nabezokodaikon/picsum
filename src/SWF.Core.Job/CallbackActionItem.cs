using System.Windows.Forms;

namespace SWF.Core.Job
{
    internal sealed class CallbackActionItem
    {
        public Control Sender { get; private set; }
        public Action CallbackAction { get; private set; }

        public CallbackActionItem(Control sender, Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            this.Sender = sender;
            this.CallbackAction = callbackAction;
        }
    }
}
