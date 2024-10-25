namespace SWF.Core.Job
{
    internal sealed class CallbackItem
    {
        public readonly SendOrPostCallback Callback;
        public readonly object? State;

        public CallbackItem(SendOrPostCallback callback, object? state)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));

            this.Callback = callback;
            this.State = state;
        }
    }
}
