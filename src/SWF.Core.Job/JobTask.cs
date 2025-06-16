namespace SWF.Core.Job
{
    public sealed partial class JobTask
        : IDisposable
    {
        private bool _disposed = false;
        private Task? _task = null;

        public JobTask()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._task?.Dispose();
                this._task = null;
            }

            this._disposed = true;
        }

        public bool IsRunning()
        {
            return this._task != null;
        }

        public void Start(Action dowork)
        {
            ArgumentNullException.ThrowIfNull(dowork, nameof(dowork));

            if (this._task != null)
            {
                throw new InvalidOperationException("タスクは既に開始しています。");
            }

            this._task ??= Task.Run(dowork);
        }

        public void Wait()
        {
            this._task?.GetAwaiter().GetResult();
        }
    }
}
