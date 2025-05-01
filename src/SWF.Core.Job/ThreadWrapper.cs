namespace SWF.Core.Job
{
    public interface IThreadWrapper
        : IDisposable
    {
        public bool IsRunning();
        public void Start(Action dowork);
        public void Wait();
    }

    public sealed partial class JobThread
        : IThreadWrapper
    {
        private bool _disposed = false;
        private Thread? _thread = null;

        public JobThread()
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
                this._thread = null;
            }

            this._disposed = true;
        }

        public bool IsRunning()
        {
            return this._thread != null;
        }

        public void Start(Action dowork)
        {
            ArgumentNullException.ThrowIfNull(dowork, nameof(dowork));

            if (this._thread != null)
            {
                throw new InvalidOperationException("スレッドは既に開始しています。");
            }

            this._thread = new(dowork.Invoke);
            this._thread.Priority = ThreadPriority.Highest;
            this._thread.Start();
        }

        public void Wait()
        {
            this._thread?.Join();
        }
    }

    public sealed partial class JobTask
        : IThreadWrapper
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
            this._task?.Wait();
        }
    }
}
