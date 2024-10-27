namespace SWF.Core.Job
{
    internal sealed class UIThreadAccessItem
    {
        public AbstractAsyncJob Job { get; private set; }
        public Action CallbackAction { get; private set; }

        public UIThreadAccessItem(AbstractAsyncJob job, Action callbackAction)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));
            ArgumentNullException.ThrowIfNull(callbackAction, nameof(callbackAction));

            this.Job = job;
            this.CallbackAction = callbackAction;
        }
    }
}
